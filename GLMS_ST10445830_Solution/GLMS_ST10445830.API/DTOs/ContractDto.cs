using GLMS_ST10445830.Models;

namespace GLMS_ST10445830.API.DTOs
{
    public class ContractDto
    {
        public int ContractId { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ContractStatus { get; set; } = string.Empty;
        public string ServiceLevel { get; set; } = string.Empty;
        public string? FilePath { get; set; }

        public ContractDto() { }

        public ContractDto(Contract contract)
        {
            ContractId = contract.ContractId;
            ClientId = contract.ClientId;
            ClientName = contract.Client?.Name ?? "Unknown";
            StartDate = contract.StartDate;
            EndDate = contract.EndDate;
            ContractStatus = contract.ContractStatus.ToString();
            ServiceLevel = contract.ServiceLevel;
            FilePath = contract.FilePath;
        }
    }
}