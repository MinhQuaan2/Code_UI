using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Request.ReportRequest
{
    public class ReportStatusFilterRequest
    {
        public Helpers.Enum.ReportStatusEnum Status { get; set; }
        public Helpers.Enum.SortingOption SortDate { get; set; }
    }
}
