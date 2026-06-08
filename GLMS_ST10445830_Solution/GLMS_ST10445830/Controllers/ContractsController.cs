using GLMS_ST10445830.API.DTOs;
using GLMS_ST10445830.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS_ST10445830.Controllers
{
   
    [Authorize]
    public class ContractsController : Controller
    {
        private readonly IContractApiService _contractApiService;
        private readonly IClientApiService _clientApiService;

        public ContractsController(IContractApiService contractApiService, IClientApiService clientApiService)
        {
            _contractApiService = contractApiService;
            _clientApiService = clientApiService;
        }

        public async Task<IActionResult> Index(ContractFilterDto? filters)
        {
            var contracts = await _contractApiService.GetContractsAsync(filters ?? new ContractFilterDto());
            return View(contracts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _contractApiService.GetContractByIdAsync(id.Value);
            if (contract == null) return NotFound();

            return View(contract);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var clients = await _clientApiService.GetClientsAsync();
            ViewData["ClientId"] = new SelectList(clients, "ClientId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateContractDto contract)
        {
            if (ModelState.IsValid)
            {
                var created = await _contractApiService.CreateContractAsync(contract);
                if (created is not null)
                {
                    TempData["Success"] = "Contract created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create contract");
            }

            var clients = await _clientApiService.GetClientsAsync();
            ViewData["ClientId"] = new SelectList(clients, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _contractApiService.GetContractByIdAsync(id.Value);
            if (contract == null) return NotFound();

            var clients = await _clientApiService.GetClientsAsync();
            ViewData["ClientId"] = new SelectList(clients, "ClientId", "Name", contract.ClientId);

            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContractDto contract)
        {
            if (id != contract.ContractId) return NotFound();

            if (ModelState.IsValid)
            {
                TempData["Success"] = "Contract updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            var clients = await _clientApiService.GetClientsAsync();
            ViewData["ClientId"] = new SelectList(clients, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _contractApiService.GetContractByIdAsync(id.Value);
            if (contract == null) return NotFound();

            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _contractApiService.DeleteContractAsync(id);
            if (deleted)
            {
                TempData["Success"] = "Contract deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Cannot delete contract with associated service requests.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}