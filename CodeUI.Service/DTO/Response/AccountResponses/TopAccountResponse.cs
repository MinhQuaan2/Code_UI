using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.AccountResponses
{
    public class TopAccountResponse
    {
        public Guid? AccountId { get; set; }

        public string? Username { get; set; }
        public string? ImageUrl { get; set; }
        public int? ElementCount { get; set; }
    }
}
