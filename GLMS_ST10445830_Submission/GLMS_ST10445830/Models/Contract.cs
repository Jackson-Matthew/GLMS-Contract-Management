using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLMS_ST10445830.Models
{
    public class Contract
    {
        public int ContractId { get; set; }

        public int ClientId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Status ContractStatus { get; set; }

        public string ServiceLevel { get; set; } = string.Empty;
    
        public string? FilePath { get; set; }

        [NotMapped]
        public IFormFile? UploadedFile { get; set; }

        public Client? Client { get; set; }

        public ICollection<ServiceRequest>? ServiceRequests { get; set; }
    }

    public enum Status
    {
        Draft,
        Active,
        Expired
    }
}