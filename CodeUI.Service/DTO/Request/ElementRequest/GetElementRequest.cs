using CodeUI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeUI.Service.Helpers.Enum;

namespace CodeUI.Service.DTO.Request.ElementRequest
{
    public class GetElementRequest
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool? IsActive { get; set; }

        /// <summary>
        /// 1-DRAFT, 2-PENDING, 3-APPROVED, 4-REJECTED
        /// </summary>
        public ElementStatusEnum? Status { get; set; }

        public string? CategoryName { get; set; }

        public Guid? OwnerId { get; set; }

        public int? CommentCount { get; set; }

        public int? LikeCount { get; set; }

        public int? Favorites { get; set; }
    }
}
