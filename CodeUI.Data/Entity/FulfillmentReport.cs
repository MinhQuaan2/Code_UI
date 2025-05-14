using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class FulfillmentReport
{
    public int Id { get; set; }

    public string ReportContent { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public int FulfillmentId { get; set; }

    public string? Status { get; set; }

    public string? Response { get; set; }

    public Guid ReporterId { get; set; }

    public virtual Fulfillment Fulfillment { get; set; } = null!;

    public virtual Account Reporter { get; set; } = null!;
}
