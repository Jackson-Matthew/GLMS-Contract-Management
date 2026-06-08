using GLMS_ST10445830.API.DTOs;
using GLMS_ST10445830.Models;
using GLMS_ST10445830.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS_ST10445830.Controllers
{
    [Authorize]
    public class ServiceRequestsController : Controller
    {
        private readonly IServiceRequestApiService _serviceRequestApiService;
        private readonly IContractApiService _contractApiService;

        public ServiceRequestsController(IServiceRequestApiService serviceRequestApiService, IContractApiService contractApiService)
        {
            _serviceRequestApiService = serviceRequestApiService;
            _contractApiService = contractApiService;
        }

        public async Task<IActionResult> Index()
        {
            var serviceRequests = await _serviceRequestApiService.GetAllAsync();
            return View(serviceRequests);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _serviceRequestApiService.GetByIdAsync(id.Value);
            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        public async Task<IActionResult> Create()
        {
            var contracts = await _contractApiService.GetContractsAsync(new ContractFilterDto());

            var contractList = contracts.Select(c => new SelectListItem
            {
                Value = c.ContractId.ToString(),
                Text = $"{c.ContractId} - {c.ClientName} ({c.ServiceLevel}) - {c.StartDate:yyyy-MM-dd} to {c.EndDate:yyyy-MM-dd}"
            }).ToList();

            contractList.Insert(0, new SelectListItem { Value = "", Text = "-- Select Contract --", Selected = true });

            ViewData["ContractId"] = contractList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest)
        {
            if (serviceRequest.ContractId.HasValue)
            {
                var contract = await _contractApiService.GetContractByIdAsync(serviceRequest.ContractId.Value);
                if (contract == null)
                {
                    ModelState.AddModelError("ContractId", "Selected contract does not exist.");
                }
            }
            else
            {
                ModelState.AddModelError("ContractId", "Please select a contract.");
            }

            if (ModelState.IsValid)
            {
                var created = await _serviceRequestApiService.CreateAsync(serviceRequest);
                if (created is not null)
                {
                    TempData["Success"] = $"Service request created! USD {serviceRequest.USDAmount:N2} = ZAR {created.ZARCost:N2}";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create service request");
            }

            await PopulateContractsDropdown(serviceRequest.ContractId);
            return View(serviceRequest);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _serviceRequestApiService.GetByIdAsync(id.Value);
            if (serviceRequest == null) return NotFound();

            await PopulateContractsDropdown(serviceRequest.ContractId);
            return View(serviceRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.ServiceRequestId) return NotFound();

            if (ModelState.IsValid)
            {
                var updated = await _serviceRequestApiService.UpdateAsync(serviceRequest);
                if (updated)
                {
                    TempData["Success"] = "Service request updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error updating service request");
            }

            await PopulateContractsDropdown(serviceRequest.ContractId);
            return View(serviceRequest);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _serviceRequestApiService.GetByIdAsync(id.Value);
            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _serviceRequestApiService.DeleteAsync(id);
            if (deleted)
            {
                TempData["Success"] = "Service request deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete service request.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateContractsDropdown(int? selectedId = null)
        {
            var contracts = await _contractApiService.GetContractsAsync(new ContractFilterDto());

            var contractList = contracts.Select(c => new SelectListItem
            {
                Value = c.ContractId.ToString(),
                Text = $"{c.ContractId} - {c.ClientName} ({c.ServiceLevel}) - {c.StartDate:yyyy-MM-dd} to {c.EndDate:yyyy-MM-dd}",
                Selected = c.ContractId == selectedId
            }).ToList();

            contractList.Insert(0, new SelectListItem { Value = "", Text = "-- Select Contract --" });

            ViewData["ContractId"] = contractList;
        }
    }
}