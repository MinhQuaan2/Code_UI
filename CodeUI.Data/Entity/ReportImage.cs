using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class ReportImage
{
    public int Id { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int ReportId { get; set; }

    public virtual Report Report { get; set; } = null!;
}
