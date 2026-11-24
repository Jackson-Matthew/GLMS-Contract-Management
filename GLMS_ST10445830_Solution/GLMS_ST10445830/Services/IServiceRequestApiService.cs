using GLMS_ST10445830.Models;

namespace GLMS_ST10445830.Services
{
    public interface IServiceRequestApiService
    {
        Task<List<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);
        Task<ServiceRequest?> CreateAsync(ServiceRequest request);
        Task<bool> UpdateAsync(ServiceRequest request);
        Task<bool> DeleteAsync(int id);
        Task<decimal?> GetExchangeRateAsync();
    }
}