using CodeUI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Request.ElementRequest
{
    public class UpdateElementRequest
    {

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? CategoryName { get; set; }
    }
}
