using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class Account
{
    public Guid Id { get; set; }

    public int? RoleId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsActive { get; set; }

    public int? ProfileId { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<AdminPoint> AdminPoints { get; set; } = new List<AdminPoint>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<DonationPackage> DonationPackages { get; set; } = new List<DonationPackage>();

    public virtual ICollection<DonationSubscription> DonationSubscriptions { get; set; } = new List<DonationSubscription>();

    public virtual ICollection<Element> Elements { get; set; } = new List<Element>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Follow> FollowFollowNavigations { get; set; } = new List<Follow>();

    public virtual ICollection<Follow> FollowFollowers { get; set; } = new List<Follow>();

    public virtual ICollection<FulfillmentReport> FulfillmentReports { get; set; } = new List<FulfillmentReport>();

    public virtual ICollection<Fulfillment> Fulfillments { get; set; } = new List<Fulfillment>();

    public virtual ICollection<LikeTable> LikeTables { get; set; } = new List<LikeTable>();

    public virtual ICollection<PointsTransaction> PointsTransactions { get; set; } = new List<PointsTransaction>();

    public virtual ICollection<PostRequest> PostRequests { get; set; } = new List<PostRequest>();

    public virtual ICollection<PremiumNote> PremiumNotes { get; set; } = new List<PremiumNote>();

    public virtual Profile? Profile { get; set; }

    public virtual ICollection<Report> ReportReporters { get; set; } = new List<Report>();

    public virtual ICollection<Report> ReportTargetAccounts { get; set; } = new List<Report>();

    public virtual ICollection<Request> RequestCreateByNavigations { get; set; } = new List<Request>();

    public virtual ICollection<Request> RequestReceiveByNavigations { get; set; } = new List<Request>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
