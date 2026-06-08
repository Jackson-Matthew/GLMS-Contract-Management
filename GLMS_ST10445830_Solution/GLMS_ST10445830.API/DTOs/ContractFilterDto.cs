using System;

namespace GLMS_ST10445830.API.DTOs
{
    public class ContractFilterDto
    {
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public string? ContractStatus { get; set; }
        public string? ServiceLevel { get; set; }
        public string? ClientName { get; set; }
    }
}