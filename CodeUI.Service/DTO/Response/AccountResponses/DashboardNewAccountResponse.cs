using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.AccountResponses
{
    public class DashboardNewAccountResponse
    {
        public int Month { get; set; }
        public int TotalNewAccount { get; set; }

        public int Diff { get; set; }
    }
}
