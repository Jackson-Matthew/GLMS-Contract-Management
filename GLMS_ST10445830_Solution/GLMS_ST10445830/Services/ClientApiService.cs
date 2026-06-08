using GLMS_ST10445830.Models;
using System.Text.Json;

namespace GLMS_ST10445830.Services
{
    public class ClientApiService : IClientApiService  // Add the interface
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ClientApiService> _logger;

        // Add constructor
        public ClientApiService(HttpClient httpClient, ILogger<ClientApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/clients");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<Client>>(json, options) ?? new List<Client>();
                }
                return new List<Client>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get clients");
                return new List<Client>();
            }
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/clients/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<Client>(json, options);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get client {id}");
                return null;
            }
        }

        public async Task<Client?> CreateClientAsync(Client client)
        {
            try
            {
                var json = JsonSerializer.Serialize(client);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/clients", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<Client>(responseJson, options);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create client");
                return null;
            }
        }

        public async Task<bool> UpdateClientAsync(Client client)
        {
            try
            {
                var json = JsonSerializer.Serialize(client);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/api/clients/{client.ClientId}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update client {client.ClientId}");
                return false;
            }
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/clients/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete client {id}");
                return false;
            }
        }
    }
}