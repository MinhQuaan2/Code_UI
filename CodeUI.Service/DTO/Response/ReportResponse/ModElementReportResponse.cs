using CodeUI.Service.DTO.Response.ReactElementResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.ReportResponse
{
    public class ModElementReportResponse
    {
        public string? CategoryName { get; set; }
        public string? UserName { get; set; }
        public int ElementId { get; set; }
        public int TotalReport { get; set; }
    }
}
