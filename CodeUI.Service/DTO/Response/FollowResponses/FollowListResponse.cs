using CodeUI.Service.DTO.Response.FollowResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.FollowResponses
{
    public class FollowListResponse
    {
        public List<FollowAccountResponse>? Followers { get; set; }
        public List<FollowAccountResponse>? Following { get; set; }
    }
}
