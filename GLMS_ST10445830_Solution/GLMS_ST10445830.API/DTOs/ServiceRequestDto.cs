using GLMS_ST10445830.Models;

namespace GLMS_ST10445830.API.DTOs
{
    public class ServiceRequestDto
    {
        public int ServiceRequestId { get; set; }
        public int? ContractId { get; set; }
        public string? Description { get; set; }
        public decimal ZARCost { get; set; }
        public decimal USDAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ContractInfo { get; set; } = string.Empty;

        public ServiceRequestDto() { }

        public ServiceRequestDto(ServiceRequest request)
        {
            ServiceRequestId = request.ServiceRequestId;
            ContractId = request.ContractId;
            Description = request.Description;
            ZARCost = request.ZARCost;
            USDAmount = request.USDAmount;
            Status = request.Status.ToString();
            ContractInfo = request.Contract != null
                ? $"Contract #{request.Contract.ContractId} - {request.Contract.ServiceLevel}"
                : "No Contract";
        }
    }
}