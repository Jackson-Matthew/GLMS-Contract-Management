using GLMS_ST10445830.Models;
using GLMS_ST10445830.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS_ST10445830.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly IClientApiService _clientApiService;

        public ClientsController(IClientApiService clientApiService)
        {
            _clientApiService = clientApiService;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _clientApiService.GetClientsAsync();
            return View(clients);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var client = await _clientApiService.GetClientByIdAsync(id.Value);
            if (client == null) return NotFound();

            return View(client);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (string.IsNullOrWhiteSpace(client.Name))
            {
                ModelState.AddModelError("Name", "Client name is required.");
            }

            if (!string.IsNullOrEmpty(client.Email) && !IsValidEmail(client.Email))
            {
                ModelState.AddModelError("Email", "Please enter a valid email address.");
            }

            if (ModelState.IsValid)
            {
                var created = await _clientApiService.CreateClientAsync(client);
                if (created is not null)
                {
                    TempData["Success"] = $"Client '{client.Name}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Unable to create client. Please check if the data is valid.");
            }
            return View(client);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = await _clientApiService.GetClientByIdAsync(id.Value);
            if (client == null) return NotFound();

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.ClientId) return NotFound();

            if (string.IsNullOrWhiteSpace(client.Name))
            {
                ModelState.AddModelError("Name", "Client name is required.");
            }

            if (!string.IsNullOrEmpty(client.Email) && !IsValidEmail(client.Email))
            {
                ModelState.AddModelError("Email", "Please enter a valid email address.");
            }

            if (ModelState.IsValid)
            {
                var updated = await _clientApiService.UpdateClientAsync(client);
                if (updated)
                {
                    TempData["Success"] = $"Client '{client.Name}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Unable to update client.");
            }

            return View(client);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = await _clientApiService.GetClientByIdAsync(id.Value);
            if (client == null) return NotFound();

            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _clientApiService.DeleteClientAsync(id);
            if (deleted)
            {
                TempData["Success"] = "Client deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Cannot delete client because they have existing contracts.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}