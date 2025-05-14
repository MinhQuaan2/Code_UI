using CodeUI.Service.DTO.Response.ProfileResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.AccountResponses
{
    public class AdminAccountResponse
    {
        public Guid Id { get; set; }

        public string? Role { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public bool? IsActive { get; set; }

        public int TotalElement { get; set; }

        public int? ProfileId { get; set; }

        public ProfileResponse? ProfileResponse { get; set; }
    }
}
