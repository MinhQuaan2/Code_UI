using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.AccountResponses
{
    public class DashboardAccountResponse
    {
        public int Month { get; set; }
        public int TotalAccount { get; set; }
        public int AccountDiff { get; set; }
    }
}
