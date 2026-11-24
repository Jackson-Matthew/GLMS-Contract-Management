using GLMS_ST10445830.Models;
using GLMS_ST10445830.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GLMS_ST10445830.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IContractApiService _contractService;
        private readonly IServiceRequestApiService _serviceRequestService;

        public HomeController(IContractApiService contractService, IServiceRequestApiService serviceRequestService)
        {
            _contractService = contractService;
            _serviceRequestService = serviceRequestService;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch the three dashboard figures from the API in parallel
            var contractsTask = _contractService.GetContractsAsync();
            var requestsTask = _serviceRequestService.GetAllAsync();
            var rateTask = _serviceRequestService.GetExchangeRateAsync();
            await Task.WhenAll(contractsTask, requestsTask, rateTask);

            ViewBag.ContractCount = contractsTask.Result.Count;
            ViewBag.ServiceRequestCount = requestsTask.Result.Count;
            ViewBag.ExchangeRate = rateTask.Result is decimal rate ? $"R {rate:0.00}" : "Unavailable";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        
    }
}
