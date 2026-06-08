using GLMS_ST10445830.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;

namespace GLMS_ST10445830.Tests.IntegrationTests
{
    public class ClientsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ClientsIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetClients_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/Clients");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetClients_ReturnsJsonData()
        {
            // Act
            var response = await _client.GetAsync("/api/Clients");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.False(string.IsNullOrEmpty(content));
        }

        [Fact]
        public async Task CreateClient_WithValidData_ReturnsCreated()
        {
            // Arrange
            var newClient = new Client
            {
                Name = "Integration Test Client",
                Email = "integration@test.com",
                PhoneNumber = "1234567890",
                Region = "Test Region"
            };
            var json = JsonSerializer.Serialize(newClient);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Clients", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseJson = await response.Content.ReadAsStringAsync();
            var createdClient = JsonSerializer.Deserialize<Client>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdClient);
            Assert.Equal("Integration Test Client", createdClient.Name);
        }

        [Fact]
        public async Task GetClientById_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/Clients/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}