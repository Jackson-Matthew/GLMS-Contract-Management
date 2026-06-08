using System.ComponentModel.DataAnnotations;

namespace GLMS_ST10445830.API.DTOs
{
    public class CreateServiceRequestDto
    {
        [Required]
        public int ContractId { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal USDAmount { get; set; }

        public string Status { get; set; } = "Pending";
    }
}