using GLMS_ST10445830.API.DTOs;
using GLMS_ST10445830.API.Repositories;
using GLMS_ST10445830.Models;
using Microsoft.AspNetCore.Mvc;

namespace GLMS_ST10445830.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly IContractRepository _contractRepo;

        public ContractsController(IContractRepository contractRepo)
        {
            _contractRepo = contractRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetContracts(
            [FromQuery] DateTime? startDateFrom,
            [FromQuery] DateTime? startDateTo,
            [FromQuery] DateTime? endDateFrom,
            [FromQuery] DateTime? endDateTo,
            [FromQuery] Status? contractStatus,
            [FromQuery] string? serviceLevel,
            [FromQuery] string? clientName)
        {
            var contracts = await _contractRepo.GetFilteredAsync(
                startDateFrom, startDateTo, endDateFrom, endDateTo,
                contractStatus, serviceLevel, clientName);
            return Ok(contracts.Select(c => new ContractDto(c)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContractDto>> GetContract(int id)
        {
            var contract = await _contractRepo.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return Ok(new ContractDto(contract));
        }

        [HttpPost]
        public async Task<ActionResult<ContractDto>> CreateContract([FromBody] CreateContractDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var contract = new Contract
            {
                ClientId = dto.ClientId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ContractStatus = Enum.Parse<Status>(dto.ContractStatus),
                ServiceLevel = dto.ServiceLevel
            };

            var created = await _contractRepo.CreateAsync(contract);
            return CreatedAtAction(nameof(GetContract), new { id = created.ContractId }, new ContractDto(created));
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateContractStatus(int id, [FromBody] UpdateContractStatusDto dto)
        {
            var exists = await _contractRepo.ExistsAsync(id);
            if (!exists) return NotFound();

            var updated = await _contractRepo.UpdateStatusAsync(id, dto.Status);
            if (!updated) return StatusCode(500, "Failed to update status");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var exists = await _contractRepo.ExistsAsync(id);
            if (!exists) return NotFound();

            var hasRequests = await _contractRepo.HasServiceRequestsAsync(id);
            if (hasRequests) return Conflict("Cannot delete contract with existing service requests");

            var deleted = await _contractRepo.DeleteAsync(id);
            if (!deleted) return StatusCode(500, "Failed to delete");

            return NoContent();
        }
    }
}