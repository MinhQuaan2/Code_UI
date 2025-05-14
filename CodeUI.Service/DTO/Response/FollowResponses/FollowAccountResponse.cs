using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.FollowResponse
{
    public class FollowAccountResponse
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public FollowProfileResponse? FollowProfileResponse { get; set; }
    }
    public class FollowProfileResponse
    {
        public string? ImageUrl { get; set; }
    }
}
