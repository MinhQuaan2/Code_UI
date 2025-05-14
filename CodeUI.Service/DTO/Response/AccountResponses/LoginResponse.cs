using CodeUI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.AccountResponses
{
    public class LoginResponse
    {
        public string? access_token { get; set; }

        public AccountResponse account { get; set; }
    }
}
