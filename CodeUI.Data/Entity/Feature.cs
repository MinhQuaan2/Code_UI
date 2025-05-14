using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class Feature
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<PackageFeature> PackageFeatures { get; set; } = new List<PackageFeature>();
}
