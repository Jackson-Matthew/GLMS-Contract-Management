using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GLMS_ST10445830.Data;
using GLMS_ST10445830.Models;
using System.IO;

namespace GLMS_ST10445830.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            DateTime? startDateFrom,
            DateTime? startDateTo,
            DateTime? endDateFrom,
            DateTime? endDateTo,
            Status? contractStatus,
            string serviceLevel,
            string clientName)
        {

            var query = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();


            if (startDateFrom.HasValue)
            {
                query = query.Where(c => c.StartDate >= startDateFrom.Value);
            }

            if (startDateTo.HasValue)
            {
                query = query.Where(c => c.StartDate <= startDateTo.Value);
            }

            if (endDateFrom.HasValue)
            {
                query = query.Where(c => c.EndDate >= endDateFrom.Value);
            }

            if (endDateTo.HasValue)
            {
                query = query.Where(c => c.EndDate <= endDateTo.Value);
            }

            if (contractStatus.HasValue)
            {
                query = query.Where(c => c.ContractStatus == contractStatus.Value);
            }

            if (!string.IsNullOrEmpty(serviceLevel))
            {
                query = query.Where(c => c.ServiceLevel.Contains(serviceLevel));
            }

            if (!string.IsNullOrEmpty(clientName))
            {
                query = query.Where(c => c.Client.Name.Contains(clientName));
            }

            var contracts = await query.ToListAsync();

            ViewBag.StartDateFrom = startDateFrom?.ToString("yyyy-MM-dd");
            ViewBag.EndDateTo = endDateTo?.ToString("yyyy-MM-dd");
            ViewBag.ContractStatus = contractStatus;
            ViewBag.ServiceLevel = serviceLevel;
            ViewBag.ClientName = clientName;


            var serviceLevels = await _context.Contracts
                .Select(c => c.ServiceLevel)
                .Distinct()
                .Where(s => !string.IsNullOrEmpty(s))
                .ToListAsync();
            ViewBag.ServiceLevels = new SelectList(serviceLevels);

            return View(contracts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.ContractId == id);

            if (contract == null) return NotFound();

            return View(contract);
        }

        public IActionResult Create()
        {
            var clients = _context.Clients.ToList();

            if (!clients.Any())
            {
                TempData["Error"] = "Please create a client first before adding contracts.";
                return RedirectToAction("Index", "Clients");
            }

            ViewData["ClientId"] = new SelectList(clients, "ClientId", "Name");

            var contract = new Contract
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
                ContractStatus = Status.Draft
            };

            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,StartDate,EndDate,ContractStatus,ServiceLevel,UploadedFile")] Contract contract)
        {

            Console.WriteLine($"Create method called. ModelState.IsValid: {ModelState.IsValid}");
            Console.WriteLine($"ClientId received: {contract.ClientId}");
            Console.WriteLine($"ServiceLevel: {contract.ServiceLevel}");
            Console.WriteLine($"StartDate: {contract.StartDate}");
            Console.WriteLine($"EndDate: {contract.EndDate}");
            Console.WriteLine($"ContractStatus: {contract.ContractStatus}");


            contract.FilePath = string.Empty;


            if (contract.ClientId == 0)
            {
                ModelState.AddModelError("ClientId", "Please select a client.");
                Console.WriteLine("Client validation failed - no client selected");
            }
            else
            {

                var clientExists = await _context.Clients.AnyAsync(c => c.ClientId == contract.ClientId);
                if (!clientExists)
                {
                    ModelState.AddModelError("ClientId", "Selected client does not exist.");
                    Console.WriteLine($"Client validation failed - ClientId {contract.ClientId} does not exist");
                }
            }

            if (contract.EndDate <= contract.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date.");
                Console.WriteLine("Date validation failed");
            }

            if (string.IsNullOrWhiteSpace(contract.ServiceLevel))
            {
                ModelState.AddModelError("ServiceLevel", "Service Level is required.");
                Console.WriteLine("ServiceLevel validation failed");
            }

            if (contract.UploadedFile != null && contract.UploadedFile.Length > 0)
            {
                if (!contract.UploadedFile.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("UploadedFile", "Only PDF files are allowed.");
                    Console.WriteLine("PDF validation failed");
                }
            }

            if (ModelState.ContainsKey("FilePath"))
            {
                ModelState.Remove("FilePath");
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (contract.UploadedFile != null && contract.UploadedFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                            Console.WriteLine("Created uploads folder");
                        }

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(contract.UploadedFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await contract.UploadedFile.CopyToAsync(stream);
                        }

                        contract.FilePath = "/files/" + fileName;
                        Console.WriteLine($"File uploaded successfully: {contract.FilePath}");
                    }

                    Console.WriteLine($"Attempting to save contract for ClientId: {contract.ClientId}");
                    _context.Add(contract);
                    await _context.SaveChangesAsync();

                    Console.WriteLine("Contract saved successfully to database");
                    TempData["Success"] = "Contract created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner error: {ex.InnerException.Message}");
                    }
                    ModelState.AddModelError("", "Unable to save contract. Please check if all data is valid.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General error: {ex.Message}");
                    ModelState.AddModelError("", $"Error saving contract: {ex.Message}");
                }
            }
            else
            {

                Console.WriteLine("ModelState is invalid. Errors:");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"- {key}: {error.ErrorMessage}");
                    }
                }
            }


            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,ClientId,StartDate,EndDate,ContractStatus,ServiceLevel,FilePath,UploadedFile")] Contract contract)
        {
            if (id != contract.ContractId) return NotFound();

            Console.WriteLine($"Edit method called for Contract ID: {id}");
            Console.WriteLine($"ClientId received: {contract.ClientId}");

            if (contract.ClientId == 0)
            {
                ModelState.AddModelError("ClientId", "Please select a client.");
                Console.WriteLine("Client validation failed - no client selected");
            }
            else
            {

                var clientExists = await _context.Clients.AnyAsync(c => c.ClientId == contract.ClientId);
                if (!clientExists)
                {
                    ModelState.AddModelError("ClientId", "Selected client does not exist.");
                    Console.WriteLine($"Client validation failed - ClientId {contract.ClientId} does not exist");
                }
            }


            if (contract.EndDate <= contract.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date.");
            }

            if (string.IsNullOrWhiteSpace(contract.ServiceLevel))
            {
                ModelState.AddModelError("ServiceLevel", "Service Level is required.");
            }

            if (contract.UploadedFile != null && contract.UploadedFile.Length > 0)
            {
                if (!contract.UploadedFile.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("UploadedFile", "Only PDF files are allowed.");
                }
            }

            if (ModelState.ContainsKey("FilePath"))
            {
                ModelState.Remove("FilePath");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingContract = await _context.Contracts.AsNoTracking()
                        .FirstOrDefaultAsync(c => c.ContractId == id);

                    if (existingContract == null) return NotFound();

                    if (contract.UploadedFile != null && contract.UploadedFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(contract.UploadedFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await contract.UploadedFile.CopyToAsync(stream);
                        }

                        contract.FilePath = "/files/" + fileName;

                        if (!string.IsNullOrEmpty(existingContract.FilePath))
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingContract.FilePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                                Console.WriteLine($"Deleted old file: {oldFilePath}");
                            }
                        }
                    }
                    else
                    {
                        contract.FilePath = existingContract.FilePath ?? string.Empty;
                    }

                    _context.Update(contract);
                    await _context.SaveChangesAsync();

                    Console.WriteLine($"Contract {id} updated successfully");
                    TempData["Success"] = "Contract updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"Concurrency error: {ex.Message}");
                    if (!ContractExists(contract.ContractId))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating contract: {ex.Message}");
                    ModelState.AddModelError("", $"Error updating contract: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Edit ModelState is invalid:");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"- {key}: {error.ErrorMessage}");
                    }
                }
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.ContractId == id);

            if (contract == null) return NotFound();

            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract != null)
            {

                var hasRequests = await _context.ServiceRequests.AnyAsync(sr => sr.ContractId == id);

                if (hasRequests)
                {
                    TempData["Error"] = "Cannot delete contract because it has associated service requests.";
                    return RedirectToAction(nameof(Index));
                }

                if (!string.IsNullOrEmpty(contract.FilePath))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", contract.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        Console.WriteLine($"Deleted file: {filePath}");
                    }
                }

                _context.Contracts.Remove(contract);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Contract deleted successfully!";
                Console.WriteLine($"Contract {id} deleted successfully");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.ContractId == id);
        }
    }
}