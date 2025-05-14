using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class Package
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int Duration { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<PackageFeature> PackageFeatures { get; set; } = new List<PackageFeature>();

    public virtual ICollection<PremiumNote> PremiumNotes { get; set; } = new List<PremiumNote>();
}
