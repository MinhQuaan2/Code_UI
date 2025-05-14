using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class Follow
{
    public int Id { get; set; }

    public Guid? FollowId { get; set; }

    public Guid? FollowerId { get; set; }

    public virtual Account? FollowNavigation { get; set; }

    public virtual Account? Follower { get; set; }
}
