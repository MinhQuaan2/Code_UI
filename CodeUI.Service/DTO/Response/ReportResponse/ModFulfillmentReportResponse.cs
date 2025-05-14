using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.ReportResponse
{
    public class ModFulfillmentReportResponse
    {
        public string? ReporterName { get; set; }
        public int FulfillmentId { get; set; }
        public string? Status { get ; set; }
        public Guid ReporterId { get; set; }
    }
}
