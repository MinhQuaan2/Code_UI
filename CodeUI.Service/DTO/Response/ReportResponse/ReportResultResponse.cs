using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.ReportResponse
{
    public class ReportResultResponse
    {
        public int Id { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public string? Response { get; set; }
        public string? ReporterEmail { get; set; }
        public string? IsReportedEmail { get; set; }
    }
}
