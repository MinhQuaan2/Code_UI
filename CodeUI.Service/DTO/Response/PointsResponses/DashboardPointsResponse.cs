using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.PointsResponses
{
    public class DashboardPointsResponse
    {
        public decimal Diff { get; set; }
        public List<CreatorPointsResponse> CreatorPointList { get; set; }
        public string Time { get; set; }
    }
}
