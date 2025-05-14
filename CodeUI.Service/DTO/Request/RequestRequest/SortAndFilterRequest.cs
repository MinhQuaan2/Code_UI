using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.DTO.Request.RequestRequest
{
    public class SortAndFilterRequest
    {
        public Helpers.Enum.SortingOption SortReward { get; set; }
        public Helpers.Enum.SortingOption SortStartDate { get; set; }
        public string? CategoryFilter { get; set; }
        public Helpers.Enum.RequestStatusEnum Status { get; set; }
        public string? CreatorName { get; set; }
        public string? RequestName { get; set; }
    }
}
