using CodeUI.Service.DTO.Response.ProfileResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.AccountResponses
{
    public class BlockedAccountResponse
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }

        public string? Email { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsBlocked { get; set; }
    }
}
