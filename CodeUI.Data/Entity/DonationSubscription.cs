using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class DonationSubscription
{
    public int Id { get; set; }

    public Guid AccountId { get; set; }

    public int PackageId { get; set; }

    public DateTime SubscribeDate { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual DonationPackage Package { get; set; } = null!;
}
