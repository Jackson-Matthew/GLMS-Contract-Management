using GLMS_ST10445830.API.DTOs;
using GLMS_ST10445830.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;

namespace GLMS_ST10445830.Tests.IntegrationTests
{
    public class ContractsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ContractsIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetContracts_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/Contracts");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateContract_WithValidData_ReturnsCreated()
        {
            // First create a client
            var newClient = new Client
            {
                Name = "Contract Test Client",
                Email = "contract@test.com",
                PhoneNumber = "1234567890",
                Region = "Test Region"
            };
            var clientJson = JsonSerializer.Serialize(newClient);
            var clientContent = new StringContent(clientJson, Encoding.UTF8, "application/json");
            var clientResponse = await _client.PostAsync("/api/Clients", clientContent);
            var clientResponseJson = await clientResponse.Content.ReadAsStringAsync();
            var createdClient = JsonSerializer.Deserialize<Client>(clientResponseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Arrange - Create a contract
            var newContract = new CreateContractDto
            {
                ClientId = createdClient.ClientId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
                ContractStatus = "Active",
                ServiceLevel = "Premium"
            };
            var json = JsonSerializer.Serialize(newContract);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Contracts", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetContractById_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/Contracts/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}