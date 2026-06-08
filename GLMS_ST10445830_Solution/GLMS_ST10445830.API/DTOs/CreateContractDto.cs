using System.ComponentModel.DataAnnotations;

namespace GLMS_ST10445830.API.DTOs
{
    public class CreateContractDto
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string ContractStatus { get; set; } = "Draft";

        [Required]
        public string ServiceLevel { get; set; } = string.Empty;
    }
}