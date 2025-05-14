using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Request.CategoryRequest
{
    public class QueryCategoryRequest
    {

        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool? IsActive { get; set; }

        public string? ImageURL { get; set; }
    }
}
