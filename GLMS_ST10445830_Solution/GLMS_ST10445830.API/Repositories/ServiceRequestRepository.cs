using GLMS_ST10445830.API.Data;
using GLMS_ST10445830.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS_ST10445830.API.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllAsync()
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id);
        }

        public async Task<IEnumerable<ServiceRequest>> GetByContractIdAsync(int contractId)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .Where(sr => sr.ContractId == contractId)
                .ToListAsync();
        }

        public async Task<ServiceRequest> CreateAsync(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();
            return serviceRequest;
        }

        public async Task<ServiceRequest> UpdateAsync(ServiceRequest serviceRequest)
        {
            _context.Entry(serviceRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return serviceRequest;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null) return false;

            _context.ServiceRequests.Remove(serviceRequest);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ServiceRequests.AnyAsync(sr => sr.ServiceRequestId == id);
        }
    }
}