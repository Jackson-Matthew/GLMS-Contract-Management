using GLMS_ST10445830.Models;

namespace GLMS_ST10445830.API.Repositories
{
    public interface IServiceRequestRepository
    {
        Task<IEnumerable<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);
        Task<IEnumerable<ServiceRequest>> GetByContractIdAsync(int contractId);
        Task<ServiceRequest> CreateAsync(ServiceRequest serviceRequest);
        Task<ServiceRequest> UpdateAsync(ServiceRequest serviceRequest);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}