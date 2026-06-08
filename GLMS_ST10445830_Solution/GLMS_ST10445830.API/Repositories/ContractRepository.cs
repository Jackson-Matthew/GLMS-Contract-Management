using GLMS_ST10445830.API.Data;
using GLMS_ST10445830.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS_ST10445830.API.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly ApplicationDbContext _context;

        public ContractRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetFilteredAsync(
            DateTime? startFrom,
            DateTime? startTo,
            DateTime? endFrom,
            DateTime? endTo,
            Status? status,
            string? serviceLevel,
            string? clientName)
        {
            var query = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            if (startFrom.HasValue)
                query = query.Where(c => c.StartDate >= startFrom.Value);

            if (startTo.HasValue)
                query = query.Where(c => c.StartDate <= startTo.Value);

            if (endFrom.HasValue)
                query = query.Where(c => c.EndDate >= endFrom.Value);

            if (endTo.HasValue)
                query = query.Where(c => c.EndDate <= endTo.Value);

            if (status.HasValue)
                query = query.Where(c => c.ContractStatus == status.Value);

            if (!string.IsNullOrEmpty(serviceLevel))
                query = query.Where(c => c.ServiceLevel.Contains(serviceLevel));

            if (!string.IsNullOrEmpty(clientName))
                query = query.Where(c => c.Client != null && c.Client.Name.Contains(clientName));

            return await query.ToListAsync();
        }

        public async Task<Contract?> GetByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ServiceRequests)
                .FirstOrDefaultAsync(c => c.ContractId == id);
        }

        public async Task<Contract> CreateAsync(Contract contract)
        {
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task<Contract> UpdateAsync(Contract contract)
        {
            _context.Entry(contract).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task<bool> UpdateStatusAsync(int id, Status newStatus)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return false;

            contract.ContractStatus = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return false;

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Contracts.AnyAsync(c => c.ContractId == id);
        }

        public async Task<bool> HasServiceRequestsAsync(int contractId)
        {
            return await _context.ServiceRequests.AnyAsync(sr => sr.ContractId == contractId);
        }
    }
}