using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class Fulfillment
{
    public int Id { get; set; }

    public string RequestDescription { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int Deadline { get; set; }

    public string? ImageUrl1 { get; set; }

    public string? ImageUrl2 { get; set; }

    public string? ImageUrl3 { get; set; }

    public bool IsActive { get; set; }

    public Guid OwnerId { get; set; }

    public int RequestId { get; set; }

    public string Status { get; set; } = null!;

    public string? Reason { get; set; }

    public virtual ICollection<FulfillmentReport> FulfillmentReports { get; set; } = new List<FulfillmentReport>();

    public virtual Account Owner { get; set; } = null!;

    public virtual Request Request { get; set; } = null!;
}
