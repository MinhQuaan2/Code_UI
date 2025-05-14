using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.PackageResponses
{
    public class PackageDashboardResponse
    {
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RevenueDiff { get; set; }
    }
}
