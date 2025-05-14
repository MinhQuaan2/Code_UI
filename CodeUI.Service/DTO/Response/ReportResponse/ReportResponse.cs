using CodeUI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.ReportResponse
{
    public class ReportResponse
    {
        public int Id { get; set; }
        public string? ReportContent { get; set; }
        public string? Reason { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public string? Response { get; set; }
        public DateTime? Timestamp { get; set; }
        public List<ReportImageResponse>? ReportImages { get; set; }
    }
    public  class ReportImageResponse
    {
        public string? ImageUrl { get; set; }
    }
}
