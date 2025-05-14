using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class AdminPoint
{
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    public Guid AccountId { get; set; }

    public string Type { get; set; } = null!;

    public decimal Amount { get; set; }

    public virtual Account Account { get; set; } = null!;
}
