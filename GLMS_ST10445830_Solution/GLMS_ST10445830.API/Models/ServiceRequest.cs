namespace GLMS_ST10445830.Models
{
    public class ServiceRequest
    {
        public int ServiceRequestId { get; set; }
        public int? ContractId { get; set; }
        public string? Description { get; set; }

        public decimal ZARCost { get; set; }
        public decimal USDAmount { get; set; }

        public RequestStatus Status { get; set; }
        public enum RequestStatus
        {
            Rejected, Pending, Approved
        }

        public Contract? Contract { get; set; }
    }
}
