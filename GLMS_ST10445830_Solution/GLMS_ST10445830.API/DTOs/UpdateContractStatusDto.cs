using GLMS_ST10445830.Models;
using System.ComponentModel.DataAnnotations;

namespace GLMS_ST10445830.API.DTOs
{
    public class UpdateContractStatusDto
    {
        [Required]
        public Status Status { get; set; }
    }
}