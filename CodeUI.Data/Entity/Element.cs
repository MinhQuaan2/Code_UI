using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class Element
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool IsActive { get; set; }

    public string Status { get; set; } = null!;

    public int CategoryId { get; set; }

    public Guid OwnerId { get; set; }

    public string? Tags { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<LikeTable> LikeTables { get; set; } = new List<LikeTable>();

    public virtual Account Owner { get; set; } = null!;

    public virtual ICollection<PostRequest> PostRequests { get; set; } = new List<PostRequest>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
