using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GLMS_ST10445830.Data;
using GLMS_ST10445830.Models;
using GLMS_ST10445830.Services;

namespace GLMS_ST10445830.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrencyConverter _converter;

        public ServiceRequestsController(ApplicationDbContext context, ICurrencyConverter converter)
        {
            _context = context;
            _converter = converter;
        }

        public async Task<IActionResult> Index()
        {
            var serviceRequests = _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client);

            return View(await serviceRequests.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(m => m.ServiceRequestId == id);

            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        public IActionResult Create()
        {
            var contracts = _context.Contracts
                .Include(c => c.Client)
                .Where(c => c.ContractStatus != Status.Expired)  
                .ToList();

            var contractList = contracts.Select(c => new SelectListItem
            {
                Value = c.ContractId.ToString(),
                Text = $"{c.ContractId} - {c.Client?.Name ?? "No Client"} ({c.ServiceLevel}) - {c.StartDate:yyyy-MM-dd} to {c.EndDate:yyyy-MM-dd}"
            }).ToList();

            contractList.Insert(0, new SelectListItem { Value = "", Text = "-- Select Contract --", Selected = true });

            ViewData["ContractId"] = contractList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ServiceRequestId,ContractId,Description,USDAmount,Status")]
            ServiceRequest serviceRequest)
        {
            ModelState.Remove("Contract");

            if (serviceRequest.ContractId.HasValue)
            {
                var contract = await _context.Contracts
                    .FirstOrDefaultAsync(c => c.ContractId == serviceRequest.ContractId.Value);

                if (contract == null)
                {
                    ModelState.AddModelError("ContractId", "Selected contract does not exist.");
                }
                else if (contract.ContractStatus == Status.Expired)
                {
                    ModelState.AddModelError("ContractId", "Cannot create a service request for an EXPIRED contract. Please renew the contract first.");
                }
            }
            else
            {
                ModelState.AddModelError("ContractId", "Please select a contract.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // USD  to ZAR conversion
                    decimal zarAmount = await _converter.ConvertUsdToZar(serviceRequest.USDAmount);
                    serviceRequest.ZARCost = zarAmount;

                    if (serviceRequest.Status == 0)
                        serviceRequest.Status = ServiceRequest.RequestStatus.Pending;

                    _context.Add(serviceRequest);
                    await _context.SaveChangesAsync();

                    TempData["Success"] =
                        $"Service request created! USD {serviceRequest.USDAmount:N2} = ZAR {zarAmount:N2}";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Currency conversion failed: {ex.Message}");
                }
            }

            await PopulateContractsDropdown(serviceRequest.ContractId);
            return View(serviceRequest);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests
                .FirstOrDefaultAsync(m => m.ServiceRequestId == id);

            if (serviceRequest == null) return NotFound();

            if (serviceRequest.ContractId.HasValue)
            {
                var contract = await _context.Contracts
                    .FirstOrDefaultAsync(c => c.ContractId == serviceRequest.ContractId.Value);

                if (contract != null && contract.ContractStatus == Status.Expired)
                {
                    TempData["Error"] = "Cannot edit service request for an EXPIRED contract.";
                    return RedirectToAction(nameof(Index));
                }
            }

            await PopulateContractsDropdown(serviceRequest.ContractId);

            return View(serviceRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ServiceRequestId,ContractId,Description,USDAmount,Status")]
            ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.ServiceRequestId)
                return NotFound();

            ModelState.Remove("Contract");

            if (serviceRequest.ContractId.HasValue)
            {
                var contract = await _context.Contracts
                    .FirstOrDefaultAsync(c => c.ContractId == serviceRequest.ContractId.Value);

                if (contract == null)
                {
                    ModelState.AddModelError("ContractId", "Selected contract does not exist.");
                }
                else if (contract.ContractStatus == Status.Expired)
                {
                    ModelState.AddModelError("ContractId", "Cannot edit service request for an EXPIRED contract.");
                }
            }
            else
            {
                ModelState.AddModelError("ContractId", "Please select a contract.");
            }

            if (ModelState.IsValid)
            {
                try
                {

                    decimal zarAmount = await _converter.ConvertUsdToZar(serviceRequest.USDAmount);
                    serviceRequest.ZARCost = zarAmount;

                    _context.Update(serviceRequest);
                    await _context.SaveChangesAsync();

                    TempData["Success"] =
                        $"Updated! USD {serviceRequest.USDAmount:N2} = ZAR {zarAmount:N2}";

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestExists(serviceRequest.ServiceRequestId))
                        return NotFound();

                    throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating service request: {ex.Message}");
                }
            }

            await PopulateContractsDropdown(serviceRequest.ContractId);
            return View(serviceRequest);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(m => m.ServiceRequestId == id);

            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest != null)
            {
                _context.ServiceRequests.Remove(serviceRequest);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Service request deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.ServiceRequestId == id);
        }

        
        private async Task PopulateContractsDropdown(int? selectedId = null)
        {
          
            var contracts = await _context.Contracts
                .Include(c => c.Client)
                .Where(c => c.ContractStatus != Status.Expired)  
                .ToListAsync();

            var contractList = contracts.Select(c => new SelectListItem
            {
                Value = c.ContractId.ToString(),
                Text = $"{c.ContractId} - {c.Client?.Name ?? "No Client"} ({c.ServiceLevel}) - {c.StartDate:yyyy-MM-dd} to {c.EndDate:yyyy-MM-dd}",
                Selected = c.ContractId == selectedId
            }).ToList();

            contractList.Insert(0, new SelectListItem { Value = "", Text = "-- Select Contract --" });

            ViewData["ContractId"] = contractList;
        }
    }
}