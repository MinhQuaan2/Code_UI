using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class PostRequest
{
    public int Id { get; set; }

    public int ElementId { get; set; }

    public Guid? ModeratorId { get; set; }

    public DateTime IssuedDate { get; set; }

    public DateTime? ReviewedDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Reason { get; set; }

    public virtual Element Element { get; set; } = null!;

    public virtual Account? Moderator { get; set; }
}
