using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GLMS_ST10445830.Data;
using GLMS_ST10445830.Models;

namespace GLMS_ST10445830.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clients.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.Contracts)
                .FirstOrDefaultAsync(m => m.ClientId == id);

            if (client == null)
            {
                return NotFound();
            }

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
                try
                {
                    _context.Add(client);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Client '{client.Name}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to create client. Please check if the data is valid.");
                }
            }
            return View(client);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.ClientId)
            {
                return NotFound();
            }

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
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Client '{client.Name}' updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(client);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.ClientId == id);

            if (client == null)
            {
                return NotFound();
            }

            var hasContracts = await _context.Contracts.AnyAsync(c => c.ClientId == id);
            ViewBag.HasContracts = hasContracts;

            return View(client);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client != null)
            {
                var hasContracts = await _context.Contracts.AnyAsync(c => c.ClientId == id);

                if (hasContracts)
                {
                    TempData["Error"] = $"Cannot delete client '{client.Name}' because they have existing contracts.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Client '{client.Name}' deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ClientId == id);
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