using CodeUI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.ReportResponse
{
    public class FulfillmentReportResponse
    {
        public int Id { get; set; }

        public string? ReportContent { get; set; }

        public string? Reason { get; set; }

        public DateTime Timestamp { get; set; }

        public int FulfillmentId { get; set; }

        public string? Status { get; set; }

        public string? Response { get; set; }

        public Guid ReporterId { get; set; }
        public int RequestId { get; set; }
    }
}
