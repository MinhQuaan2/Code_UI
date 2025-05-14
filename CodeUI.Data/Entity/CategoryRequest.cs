using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class CategoryRequest
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public DateTime IssuedDate { get; set; }

    public DateTime? ReviewedDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Reason { get; set; }

    public string Type { get; set; } = null!;

    public string UpdateName { get; set; } = null!;

    public string UpdateDescription { get; set; } = null!;

    public string? UpdateImageUrl { get; set; }

    public virtual Category Category { get; set; } = null!;
}
