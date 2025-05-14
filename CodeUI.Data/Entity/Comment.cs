using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class Comment
{
    public int Id { get; set; }

    public string? CommentContent { get; set; }

    public DateTime? Timestamp { get; set; }

    public int ElementId { get; set; }

    public Guid AccountId { get; set; }

    public int? RootCommentId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Element Element { get; set; } = null!;

    public virtual ICollection<Comment> InverseRootComment { get; set; } = new List<Comment>();

    public virtual Comment? RootComment { get; set; }
}
