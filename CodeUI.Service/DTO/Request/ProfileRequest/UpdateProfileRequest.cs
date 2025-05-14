using CodeUI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Request.ProfileRequest
{
    public class UpdateProfileRequest
    {
        public string? Username { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Phone { get; set; }

        public string? Gender { get; set; }

        public string? Location { get; set; }

        public string? Description { get; set; }

        public decimal? Wallet { get; set; }

        public string? ImageUrl { get; set; }
    }
}
