using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.StaffResponses
{
    public class StaffResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;

        public string Email { get; set; }
        public string Phone { get; set; }
        public string Fullname { get; set; }

        public string Token { get; set; } = null!;
    }
}
