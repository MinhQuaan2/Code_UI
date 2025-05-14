using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class Report
{
    public int Id { get; set; }

    public string ReportContent { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public DateTime? Timestamp { get; set; }

    public int? ElementId { get; set; }

    public Guid ReporterId { get; set; }

    public Guid? TargetAccountId { get; set; }

    public string Status { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Response { get; set; }

    public virtual Element? Element { get; set; }

    public virtual ICollection<ReportImage> ReportImages { get; set; } = new List<ReportImage>();

    public virtual Account Reporter { get; set; } = null!;

    public virtual Account? TargetAccount { get; set; }
}
