using GLMS_ST10445830.API.DTOs;
using System.Text.Json;

namespace GLMS_ST10445830.Services
{
    public class ContractApiService : IContractApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ContractApiService> _logger;

        public ContractApiService(HttpClient httpClient, ILogger<ContractApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        private string BuildQueryString(ContractFilterDto? filters)
        {
            if (filters == null) return string.Empty;

            var query = new List<string>();

            if (filters.StartDateFrom.HasValue)
                query.Add($"startDateFrom={filters.StartDateFrom.Value:yyyy-MM-dd}");
            if (filters.StartDateTo.HasValue)
                query.Add($"startDateTo={filters.StartDateTo.Value:yyyy-MM-dd}");
            if (filters.EndDateFrom.HasValue)
                query.Add($"endDateFrom={filters.EndDateFrom.Value:yyyy-MM-dd}");
            if (filters.EndDateTo.HasValue)
                query.Add($"endDateTo={filters.EndDateTo.Value:yyyy-MM-dd}");
            if (!string.IsNullOrEmpty(filters.ContractStatus))
                query.Add($"contractStatus={filters.ContractStatus}");
            if (!string.IsNullOrEmpty(filters.ServiceLevel))
                query.Add($"serviceLevel={Uri.EscapeDataString(filters.ServiceLevel)}");
            if (!string.IsNullOrEmpty(filters.ClientName))
                query.Add($"clientName={Uri.EscapeDataString(filters.ClientName)}");

            return query.Any() ? $"?{string.Join("&", query)}" : "";
        }

        public async Task<List<ContractDto>> GetContractsAsync(ContractFilterDto? filters = null)
        {
            try
            {
                var queryString = BuildQueryString(filters);
                var response = await _httpClient.GetAsync($"/api/contracts{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<ContractDto>>(json, options) ?? new List<ContractDto>();
                }

                return new List<ContractDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get contracts");
                return new List<ContractDto>();
            }
        }

        public async Task<ContractDto?> GetContractByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/contracts/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<ContractDto>(json, options);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get contract {id}");
                return null;
            }
        }

        public async Task<ContractDto?> CreateContractAsync(CreateContractDto contract)
        {
            try
            {
                var json = JsonSerializer.Serialize(contract);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/contracts", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<ContractDto>(responseJson, options);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create contract");
                return null;
            }
        }

        public async Task<bool> UpdateContractStatusAsync(int id, string status)  // FIXED: string not Status
        {
            try
            {
                var updateDto = new { status = status };
                var json = JsonSerializer.Serialize(updateDto);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync($"/api/contracts/{id}/status", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update contract {id} status");
                return false;
            }
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/contracts/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete contract {id}");
                return false;
            }
        }
    }
}