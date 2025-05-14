using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool? IsActive { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<CategoryRequest> CategoryRequests { get; set; } = new List<CategoryRequest>();

    public virtual ICollection<Element> Elements { get; set; } = new List<Element>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
