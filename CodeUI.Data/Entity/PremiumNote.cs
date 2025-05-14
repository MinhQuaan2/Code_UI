using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class PremiumNote
{
    public int Id { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int PackageId { get; set; }

    public Guid AccountId { get; set; }

    public bool IsActive { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Package Package { get; set; } = null!;
}
