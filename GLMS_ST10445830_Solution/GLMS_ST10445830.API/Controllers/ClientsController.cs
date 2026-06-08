using GLMS_ST10445830.API.DTOs;
using GLMS_ST10445830.API.Repositories;
using GLMS_ST10445830.Models;
using Microsoft.AspNetCore.Mvc;

namespace GLMS_ST10445830.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepo;

        public ClientsController(IClientRepository clientRepo)
        {
            _clientRepo = clientRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            var clients = await _clientRepo.GetAllAsync();
            return Ok(clients.Select(c => new ClientDto(c)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> GetClient(int id)
        {
            var client = await _clientRepo.GetByIdAsync(id);
            if (client == null) return NotFound();
            return Ok(new ClientDto(client));
        }

        [HttpPost]
        public async Task<ActionResult<ClientDto>> CreateClient([FromBody] Client client)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _clientRepo.CreateAsync(client);
            return CreatedAtAction(nameof(GetClient), new { id = created.ClientId }, new ClientDto(created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] Client client)
        {
            if (id != client.ClientId) return BadRequest("ID mismatch");

            var exists = await _clientRepo.ExistsAsync(id);
            if (!exists) return NotFound();

            await _clientRepo.UpdateAsync(client);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var exists = await _clientRepo.ExistsAsync(id);
            if (!exists) return NotFound();

            var hasContracts = await _clientRepo.HasContractsAsync(id);
            if (hasContracts) return Conflict("Cannot delete client with existing contracts");

            var deleted = await _clientRepo.DeleteAsync(id);
            if (!deleted) return StatusCode(500, "Failed to delete");

            return NoContent();
        }
    }
}