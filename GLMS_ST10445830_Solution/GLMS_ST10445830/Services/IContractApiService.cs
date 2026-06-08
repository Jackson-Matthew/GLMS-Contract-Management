using GLMS_ST10445830.API.DTOs;

namespace GLMS_ST10445830.Services
{
    public interface IContractApiService
    {
        Task<List<ContractDto>> GetContractsAsync(ContractFilterDto? filters = null);
        Task<ContractDto?> GetContractByIdAsync(int id);
        Task<ContractDto?> CreateContractAsync(CreateContractDto contract);
        Task<bool> UpdateContractStatusAsync(int id, string status);  // Use string, not Status
        Task<bool> DeleteContractAsync(int id);
    }
}