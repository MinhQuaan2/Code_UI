using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class PackageFeature
{
    public int Id { get; set; }

    public int? PackageId { get; set; }

    public int? FeatureId { get; set; }

    public string? Config { get; set; }

    public int? Number { get; set; }

    public string? Unit { get; set; }

    public virtual Feature? Feature { get; set; }

    public virtual Package? Package { get; set; }
}
