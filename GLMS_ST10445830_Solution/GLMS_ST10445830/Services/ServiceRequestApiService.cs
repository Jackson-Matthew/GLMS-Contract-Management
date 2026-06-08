using System.Text;
using System.Text.Json;
using GLMS_ST10445830.Models;

namespace GLMS_ST10445830.Services
{
    public class ServiceRequestApiService : IServiceRequestApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ServiceRequestApiService> _logger;

        public ServiceRequestApiService(HttpClient httpClient, ILogger<ServiceRequestApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<ServiceRequest>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/ServiceRequests");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    // Deserialize to a temporary DTO first
                    var tempRequests = JsonSerializer.Deserialize<List<ServiceRequestDto>>(json, options);
                    if (tempRequests != null)
                    {
                        // Convert DTOs to ServiceRequest models
                        return tempRequests.Select(dto => new ServiceRequest
                        {
                            ServiceRequestId = dto.ServiceRequestId,
                            ContractId = dto.ContractId,
                            Description = dto.Description,
                            USDAmount = dto.UsdAmount,
                            ZARCost = dto.ZarCost,
                            Status = ConvertStringToStatus(dto.Status)
                        }).ToList();
                    }
                }
                return new List<ServiceRequest>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get service requests");
                return new List<ServiceRequest>();
            }
        }

        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/ServiceRequests/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var dto = JsonSerializer.Deserialize<ServiceRequestDto>(json, options);
                    if (dto != null)
                    {
                        return new ServiceRequest
                        {
                            ServiceRequestId = dto.ServiceRequestId,
                            ContractId = dto.ContractId,
                            Description = dto.Description,
                            USDAmount = dto.UsdAmount,
                            ZARCost = dto.ZarCost,
                            Status = ConvertStringToStatus(dto.Status)
                        };
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get service request {id}");
                return null;
            }
        }

        public async Task<ServiceRequest?> CreateAsync(ServiceRequest request)
        {
            try
            {
                var createRequest = new
                {
                    contractId = request.ContractId,
                    description = request.Description,
                    usdAmount = request.USDAmount,
                    status = request.Status.ToString()  // Convert enum to string
                };

                var json = JsonSerializer.Serialize(createRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/ServiceRequests", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var dto = JsonSerializer.Deserialize<ServiceRequestDto>(responseJson, options);

                    if (dto != null)
                    {
                        return new ServiceRequest
                        {
                            ServiceRequestId = dto.ServiceRequestId,
                            ContractId = dto.ContractId,
                            Description = dto.Description,
                            USDAmount = dto.UsdAmount,
                            ZARCost = dto.ZarCost,
                            Status = ConvertStringToStatus(dto.Status)
                        };
                    }
                }

                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"API Error {response.StatusCode}: {error}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create service request");
                return null;
            }
        }

        public async Task<bool> UpdateAsync(ServiceRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/api/ServiceRequests/{request.ServiceRequestId}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update service request {request.ServiceRequestId}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/ServiceRequests/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete service request {id}");
                return false;
            }
        }

        private ServiceRequest.RequestStatus ConvertStringToStatus(string status)
        {
            return status?.ToLower() switch
            {
                "pending" => ServiceRequest.RequestStatus.Pending,
                "approved" => ServiceRequest.RequestStatus.Approved,
                "rejected" => ServiceRequest.RequestStatus.Rejected,
                _ => ServiceRequest.RequestStatus.Pending
            };
        }

        // DTO for deserialization from API
        private class ServiceRequestDto
        {
            public int ServiceRequestId { get; set; }
            public int? ContractId { get; set; }
            public string? Description { get; set; }
            public decimal ZarCost { get; set; }
            public decimal UsdAmount { get; set; }
            public string Status { get; set; } = string.Empty;
            public string ContractInfo { get; set; } = string.Empty;
        }
    }
}