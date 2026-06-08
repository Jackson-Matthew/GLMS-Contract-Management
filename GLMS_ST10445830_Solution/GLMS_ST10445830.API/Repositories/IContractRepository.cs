using GLMS_ST10445830.Models;

namespace GLMS_ST10445830.API.Repositories
{
    public interface IContractRepository
    {
        Task<IEnumerable<Contract>> GetAllAsync();
        Task<IEnumerable<Contract>> GetFilteredAsync(
            DateTime? startFrom,
            DateTime? startTo,
            DateTime? endFrom,
            DateTime? endTo,
            Status? status,
            string? serviceLevel,
            string? clientName);
        Task<Contract?> GetByIdAsync(int id);
        Task<Contract> CreateAsync(Contract contract);
        Task<Contract> UpdateAsync(Contract contract);
        Task<bool> UpdateStatusAsync(int id, Status newStatus);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> HasServiceRequestsAsync(int contractId);
    }
}