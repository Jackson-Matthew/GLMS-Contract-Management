using GLMS_ST10445830.API.DTOs;
using GLMS_ST10445830.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;

namespace GLMS_ST10445830.Tests.IntegrationTests
{
    public class ServiceRequestsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ServiceRequestsIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetServiceRequests_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/ServiceRequests");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateServiceRequest_WithValidData_ReturnsCreated()
        {
            // First create a client
            var newClient = new Client
            {
                Name = "SR Test Client",
                Email = "sr@test.com",
                PhoneNumber = "1234567890",
                Region = "Test Region"
            };
            var clientJson = JsonSerializer.Serialize(newClient);
            var clientContent = new StringContent(clientJson, Encoding.UTF8, "application/json");
            var clientResponse = await _client.PostAsync("/api/Clients", clientContent);
            var clientResponseJson = await clientResponse.Content.ReadAsStringAsync();
            var createdClient = JsonSerializer.Deserialize<Client>(clientResponseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Then create a contract
            var newContract = new CreateContractDto
            {
                ClientId = createdClient.ClientId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
                ContractStatus = "Active",
                ServiceLevel = "Premium"
            };
            var contractJson = JsonSerializer.Serialize(newContract);
            var contractContent = new StringContent(contractJson, Encoding.UTF8, "application/json");
            var contractResponse = await _client.PostAsync("/api/Contracts", contractContent);
            var contractResponseJson = await contractResponse.Content.ReadAsStringAsync();
            var createdContract = JsonSerializer.Deserialize<ContractDto>(contractResponseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Arrange - Create a service request
            var newRequest = new
            {
                contractId = createdContract.ContractId,
                description = "Test service request",
                usdAmount = 100.00m,
                status = "Pending"
            };
            var json = JsonSerializer.Serialize(newRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/ServiceRequests", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}