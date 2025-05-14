using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.ReportResponse
{
    public class ModAccountReportResponse
    {
        public int TotalReport { get; set; }
        public string? UserName { get; set; }
        public Guid? AccountId { get; set; }
    }
}
