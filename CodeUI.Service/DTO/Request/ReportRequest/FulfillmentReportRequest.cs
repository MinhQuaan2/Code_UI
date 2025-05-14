using CodeUI.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Request.ReportRequest
{
    public class FulfillmentReportRequest
    {
        [Required]
        public string? ReportContent { get; set; }
        [Required]
        public string? Reason { get; set; }
    }
}
