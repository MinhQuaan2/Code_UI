using CodeUI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeUI.Service.DTO.Response.FollowResponse;
using CodeUI.Service.DTO.Response.ProfileResponses;

namespace CodeUI.Service.DTO.Response.AccountResponses
{
    public class AccountResponse
    {
        public Guid Id { get; set; }

        public string? Role { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public bool? IsActive { get; set; }

        public int? ProfileId { get; set; }

        public List<FollowResponse.FollowResponse>? Followers { get; set; }

        public List<FollowResponse.FollowResponse>? Followings { get; set; }

        public ProfileResponse? ProfileResponse { get; set; }
    }
}
