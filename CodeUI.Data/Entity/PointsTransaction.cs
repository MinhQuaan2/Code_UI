using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class PointsTransaction
{
    public int Id { get; set; }

    public Guid AccountId { get; set; }

    public decimal Amount { get; set; }

    public string Type { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public virtual Account Account { get; set; } = null!;
}
