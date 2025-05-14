using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class DonationPackage
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public Guid OwnerId { get; set; }

    public bool IsActive { get; set; }

    public int SubscribeLimit { get; set; }

    public int DonationPackageDetailId { get; set; }

    public virtual DonationPackageDetail DonationPackageDetail { get; set; } = null!;

    public virtual ICollection<DonationSubscription> DonationSubscriptions { get; set; } = new List<DonationSubscription>();

    public virtual Account Owner { get; set; } = null!;
}
