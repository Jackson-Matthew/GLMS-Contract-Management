using GLMS_ST10445830.API.DTOs;
using GLMS_ST10445830.API.Repositories;
using GLMS_ST10445830.Models;
using GLMS_ST10445830.Services;
using Microsoft.AspNetCore.Mvc;
using GLMS_ST10445830.API.Services;

namespace GLMS_ST10445830.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestRepository _serviceRequestRepo;
        private readonly IContractRepository _contractRepo;
        private readonly ICurrencyConverter _currencyConverter;  // ADD THIS

        public ServiceRequestsController(
            IServiceRequestRepository serviceRequestRepo,
            IContractRepository contractRepo,
            ICurrencyConverter currencyConverter)  // ADD THIS PARAMETER
        {
            _serviceRequestRepo = serviceRequestRepo;
            _contractRepo = contractRepo;
            _currencyConverter = currencyConverter;  // ADD THIS
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> GetServiceRequests()
        {
            var requests = await _serviceRequestRepo.GetAllAsync();
            return Ok(requests.Select(r => new ServiceRequestDto(r)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceRequestDto>> GetServiceRequest(int id)
        {
            var request = await _serviceRequestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();
            return Ok(new ServiceRequestDto(request));
        }

        // Returns the current USD -> ZAR rate (value of 1 USD in ZAR) using the configured converter
        [HttpGet("exchange-rate")]
        public async Task<ActionResult<decimal>> GetExchangeRate()
        {
            var rate = await _currencyConverter.ConvertUsdToZar(1m);
            return Ok(rate);
        }

        [HttpGet("contract/{contractId}")]
        public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> GetServiceRequestsByContract(int contractId)
        {
            var requests = await _serviceRequestRepo.GetByContractIdAsync(contractId);
            return Ok(requests.Select(r => new ServiceRequestDto(r)));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceRequestDto>> CreateServiceRequest([FromBody] CreateServiceRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var contractExists = await _contractRepo.ExistsAsync(dto.ContractId);
            if (!contractExists) return BadRequest("Contract does not exist");

            var contract = await _contractRepo.GetByIdAsync(dto.ContractId);
            if (contract != null && contract.ContractStatus == Status.Expired)
                return BadRequest("Cannot create service request for expired contract");

            // CALCULATE ZAR COST - ADD THIS
            decimal zarCost = 0;
            if (dto.USDAmount > 0)
            {
                zarCost = await _currencyConverter.ConvertUsdToZar(dto.USDAmount);
            }

            var request = new ServiceRequest
            {
                ContractId = dto.ContractId,
                Description = dto.Description,
                USDAmount = dto.USDAmount,
                ZARCost = zarCost,  // ADD THIS LINE
                Status = Enum.Parse<ServiceRequest.RequestStatus>(dto.Status)
            };

            var created = await _serviceRequestRepo.CreateAsync(request);
            return CreatedAtAction(nameof(GetServiceRequest), new { id = created.ServiceRequestId }, new ServiceRequestDto(created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceRequest(int id, [FromBody] ServiceRequest request)
        {
            if (id != request.ServiceRequestId) return BadRequest("ID mismatch");

            var exists = await _serviceRequestRepo.ExistsAsync(id);
            if (!exists) return NotFound();

            await _serviceRequestRepo.UpdateAsync(request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            var exists = await _serviceRequestRepo.ExistsAsync(id);
            if (!exists) return NotFound();

            var deleted = await _serviceRequestRepo.DeleteAsync(id);
            if (!deleted) return StatusCode(500, "Failed to delete");

            return NoContent();
        }
    }
}