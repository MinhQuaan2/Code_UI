using CodeUI.Service.Attributes;
using CodeUI.Service.DTO.Response.ProfileResponses;
using CodeUI.Service.DTO.Response.ReactElementResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Response.ElementResponses
{
    public class SimpleElementResponse
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool? IsActive { get; set; }

        public string? Status { get; set; }

        public string? CategoryName { get; set; }

        public string? OwnerUsername { get; set; }

        public int? CommentCount { get; set; }

        public int? LikeCount { get; set; }

        public int? Favorites { get; set; }

        public bool? IsLiked { get; set; }

        public bool? IsFavorite { get; set; }

        [Child]
        public virtual ProfileResponse? ProfileResponse { get; set; }
    }
}
