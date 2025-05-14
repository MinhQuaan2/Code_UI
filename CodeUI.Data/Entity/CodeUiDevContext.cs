using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CodeUI.Data.Entity;

public partial class CodeUiDevContext : DbContext
{
    public CodeUiDevContext()
    {
    }

    public CodeUiDevContext(DbContextOptions<CodeUiDevContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AdminPoint> AdminPoints { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryRequest> CategoryRequests { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<DonationBenefit> DonationBenefits { get; set; }

    public virtual DbSet<DonationDetailBenefit> DonationDetailBenefits { get; set; }

    public virtual DbSet<DonationPackage> DonationPackages { get; set; }

    public virtual DbSet<DonationPackageDetail> DonationPackageDetails { get; set; }

    public virtual DbSet<DonationSubscription> DonationSubscriptions { get; set; }

    public virtual DbSet<Element> Elements { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<Follow> Follows { get; set; }

    public virtual DbSet<Fulfillment> Fulfillments { get; set; }

    public virtual DbSet<FulfillmentReport> FulfillmentReports { get; set; }

    public virtual DbSet<LikeTable> LikeTables { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<PackageFeature> PackageFeatures { get; set; }

    public virtual DbSet<PointsTransaction> PointsTransactions { get; set; }

    public virtual DbSet<PostRequest> PostRequests { get; set; }

    public virtual DbSet<PremiumNote> PremiumNotes { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<ReportImage> ReportImages { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StaffAccount> StaffAccounts { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=54.151.179.184;Database=CodeUI_dev;User ID=sa;Password=QuanNM_0516;MultipleActiveResultSets=true;Integrated Security=true;Trusted_Connection=False;Encrypt=True;TrustServerCertificate=True", x => x.UseNetTopologySuite());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Account_1");

            entity.ToTable("Account");

            entity.HasIndex(e => e.ProfileId, "UQ__Account__290C88E5F9B7EC4B").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E40842F6EE").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("date");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(12);
            entity.Property(e => e.UpdateDate).HasColumnType("date");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Profile).WithOne(p => p.Account)
                .HasForeignKey<Account>(d => d.ProfileId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Account__Profile__76969D2E");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<AdminPoint>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.Timestamp).HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(30);

            entity.HasOne(d => d.Account).WithMany(p => p.AdminPoints)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AdminPoints_Account");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.HasIndex(e => e.Name, "UQ__Category__737584F666556076").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasColumnName("ImageURL");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<CategoryRequest>(entity =>
        {
            entity.ToTable("CategoryRequest");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.IssuedDate).HasColumnType("date");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.ReviewedDate).HasColumnType("date");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdateDescription).HasMaxLength(500);
            entity.Property(e => e.UpdateName).HasMaxLength(50);

            entity.HasOne(d => d.Category).WithMany(p => p.CategoryRequests)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CategoryRequest_Category");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comment");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.CommentContent).HasMaxLength(150);
            entity.Property(e => e.ElementId).HasColumnName("ElementID");
            entity.Property(e => e.RootCommentId).HasColumnName("RootCommentID");
            entity.Property(e => e.Timestamp).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Comments)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Comment_Account");

            entity.HasOne(d => d.Element).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ElementId)
                .HasConstraintName("FK_Comment_Element");

            entity.HasOne(d => d.RootComment).WithMany(p => p.InverseRootComment)
                .HasForeignKey(d => d.RootCommentId)
                .HasConstraintName("FK_Comment_RootComment");
        });

        modelBuilder.Entity<DonationBenefit>(entity =>
        {
            entity.ToTable("DonationBenefit");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.Title).HasMaxLength(30);
        });

        modelBuilder.Entity<DonationDetailBenefit>(entity =>
        {
            entity.ToTable("DonationDetail_Benefit");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Config).HasMaxLength(100);
            entity.Property(e => e.DonationBenefitId).HasColumnName("DonationBenefitID");
            entity.Property(e => e.DonationDetailId).HasColumnName("DonationDetailID");
            entity.Property(e => e.Unit).HasMaxLength(100);

            entity.HasOne(d => d.DonationBenefit).WithMany(p => p.DonationDetailBenefits)
                .HasForeignKey(d => d.DonationBenefitId)
                .HasConstraintName("FK_DonationDetail_Benefit_DonationBenefit");

            entity.HasOne(d => d.DonationDetail).WithMany(p => p.DonationDetailBenefits)
                .HasForeignKey(d => d.DonationDetailId)
                .HasConstraintName("FK_DonationDetail_Benefit_DonationPackageDetail");
        });

        modelBuilder.Entity<DonationPackage>(entity =>
        {
            entity.ToTable("DonationPackage");

            entity.HasIndex(e => e.DonationPackageDetailId, "uc_DonationPackageDetailID").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.DonationPackageDetailId).HasColumnName("DonationPackageDetailID");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.Title).HasMaxLength(30);

            entity.HasOne(d => d.DonationPackageDetail).WithOne(p => p.DonationPackage)
                .HasForeignKey<DonationPackage>(d => d.DonationPackageDetailId)
                .HasConstraintName("FK_DonationPackage_DonationPackageDetail");

            entity.HasOne(d => d.Owner).WithMany(p => p.DonationPackages)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_DonationPackage_Account");
        });

        modelBuilder.Entity<DonationPackageDetail>(entity =>
        {
            entity.ToTable("DonationPackageDetail");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("date");
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.UpdateDate).HasColumnType("date");
        });

        modelBuilder.Entity<DonationSubscription>(entity =>
        {
            entity.ToTable("DonationSubscription");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.PackageId).HasColumnName("PackageID");
            entity.Property(e => e.SubscribeDate).HasColumnType("date");

            entity.HasOne(d => d.Account).WithMany(p => p.DonationSubscriptions)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_DonationSubscription_Account");

            entity.HasOne(d => d.Package).WithMany(p => p.DonationSubscriptions)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonationSubscription_DonationPackage");
        });

        modelBuilder.Entity<Element>(entity =>
        {
            entity.ToTable("Element");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreateDate).HasColumnType("date");
            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Tags).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.UpdateDate).HasColumnType("date");

            entity.HasOne(d => d.Category).WithMany(p => p.Elements)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Element_Category");

            entity.HasOne(d => d.Owner).WithMany(p => p.Elements)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Element_Account");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.ToTable("Favorite");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.ElementId).HasColumnName("ElementID");
            entity.Property(e => e.Timestamp).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Favorite_Account");

            entity.HasOne(d => d.Element).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.ElementId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Favorite_Element");
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.ToTable("Feature");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Follow>(entity =>
        {
            entity.ToTable("Follow");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FollowId)
                .HasDefaultValueSql("('00000000-0000-0000-0000-000000000000')")
                .HasColumnName("FollowID");
            entity.Property(e => e.FollowerId)
                .HasDefaultValueSql("('00000000-0000-0000-0000-000000000000')")
                .HasColumnName("FollowerID");

            entity.HasOne(d => d.FollowNavigation).WithMany(p => p.FollowFollowNavigations)
                .HasForeignKey(d => d.FollowId)
                .HasConstraintName("FK_Follow_Account");

            entity.HasOne(d => d.Follower).WithMany(p => p.FollowFollowers)
                .HasForeignKey(d => d.FollowerId)
                .HasConstraintName("FK_Follow_Account1");
        });

        modelBuilder.Entity<Fulfillment>(entity =>
        {
            entity.ToTable("Fulfillment");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.RequestDescription).HasMaxLength(500);
            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Owner).WithMany(p => p.Fulfillments)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_Fulfillment_Account");

            entity.HasOne(d => d.Request).WithMany(p => p.Fulfillments)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_Fulfillment_Request");
        });

        modelBuilder.Entity<FulfillmentReport>(entity =>
        {
            entity.ToTable("FulfillmentReport");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FulfillmentId).HasColumnName("FulfillmentID");
            entity.Property(e => e.Reason).HasMaxLength(200);
            entity.Property(e => e.ReportContent).HasMaxLength(500);
            entity.Property(e => e.ReporterId).HasColumnName("ReporterID");
            entity.Property(e => e.Response).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Timestamp).HasColumnType("datetime");

            entity.HasOne(d => d.Fulfillment).WithMany(p => p.FulfillmentReports)
                .HasForeignKey(d => d.FulfillmentId)
                .HasConstraintName("FK_FulfillmentReport_Fulfillment");

            entity.HasOne(d => d.Reporter).WithMany(p => p.FulfillmentReports)
                .HasForeignKey(d => d.ReporterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FulfillmentReport_Account");
        });

        modelBuilder.Entity<LikeTable>(entity =>
        {
            entity.ToTable("LikeTable");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.ElementId).HasColumnName("ElementID");

            entity.HasOne(d => d.Account).WithMany(p => p.LikeTables)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_LikeTable_Account");

            entity.HasOne(d => d.Element).WithMany(p => p.LikeTables)
                .HasForeignKey(d => d.ElementId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_LikeTable_Element");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.ToTable("Package");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt).HasColumnType("date");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("money");
        });

        modelBuilder.Entity<PackageFeature>(entity =>
        {
            entity.ToTable("PackageFeature");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Config).HasMaxLength(100);
            entity.Property(e => e.FeatureId).HasColumnName("FeatureID");
            entity.Property(e => e.PackageId).HasColumnName("PackageID");
            entity.Property(e => e.Unit).HasMaxLength(100);

            entity.HasOne(d => d.Feature).WithMany(p => p.PackageFeatures)
                .HasForeignKey(d => d.FeatureId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PackageFeature_Feature");

            entity.HasOne(d => d.Package).WithMany(p => p.PackageFeatures)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PackageFeature_Package");
        });

        modelBuilder.Entity<PointsTransaction>(entity =>
        {
            entity.ToTable("PointsTransaction");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.Timestamp).HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(30);

            entity.HasOne(d => d.Account).WithMany(p => p.PointsTransactions)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PointsTransaction_Account");
        });

        modelBuilder.Entity<PostRequest>(entity =>
        {
            entity.ToTable("PostRequest");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ElementId).HasColumnName("ElementID");
            entity.Property(e => e.IssuedDate).HasColumnType("date");
            entity.Property(e => e.ModeratorId).HasColumnName("ModeratorID");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.ReviewedDate).HasColumnType("date");
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Element).WithMany(p => p.PostRequests)
                .HasForeignKey(d => d.ElementId)
                .HasConstraintName("FK_PostRequest_Element");

            entity.HasOne(d => d.Moderator).WithMany(p => p.PostRequests)
                .HasForeignKey(d => d.ModeratorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PostRequest_Account");
        });

        modelBuilder.Entity<PremiumNote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_PackageDetail");

            entity.ToTable("PremiumNote");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.PackageId).HasColumnName("PackageID");
            entity.Property(e => e.StartDate).HasColumnType("date");

            entity.HasOne(d => d.Account).WithMany(p => p.PremiumNotes)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_PremiumNote_Account");

            entity.HasOne(d => d.Package).WithMany(p => p.PremiumNotes)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK_PremiumNote_Package");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.ToTable("Profile");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DateOfBirth).HasColumnType("date");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(20);
            entity.Property(e => e.Gender).HasMaxLength(15);
            entity.Property(e => e.LastName).HasMaxLength(20);
            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.Wallet)
                .HasDefaultValueSql("((0))")
                .HasColumnType("money");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.ToTable("Report");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ElementId).HasColumnName("ElementID");
            entity.Property(e => e.Reason).HasMaxLength(200);
            entity.Property(e => e.ReportContent).HasMaxLength(500);
            entity.Property(e => e.ReporterId).HasColumnName("ReporterID");
            entity.Property(e => e.Response).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TargetAccountId).HasColumnName("TargetAccountID");
            entity.Property(e => e.Timestamp).HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(20);

            entity.HasOne(d => d.Element).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ElementId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Report_Element");

            entity.HasOne(d => d.Reporter).WithMany(p => p.ReportReporters)
                .HasForeignKey(d => d.ReporterId)
                .HasConstraintName("FK_Report_Account");

            entity.HasOne(d => d.TargetAccount).WithMany(p => p.ReportTargetAccounts)
                .HasForeignKey(d => d.TargetAccountId)
                .HasConstraintName("FK_Report_Account1");
        });

        modelBuilder.Entity<ReportImage>(entity =>
        {
            entity.ToTable("ReportImage");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ReportId).HasColumnName("ReportID");

            entity.HasOne(d => d.Report).WithMany(p => p.ReportImages)
                .HasForeignKey(d => d.ReportId)
                .HasConstraintName("FK_ReportImage_Report");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.ToTable("Request");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Deposit).HasColumnType("money");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RequestDescription).HasMaxLength(2000);
            entity.Property(e => e.Reward).HasColumnType("money");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TypeCss)
                .HasMaxLength(50)
                .HasColumnName("TypeCSS");

            entity.HasOne(d => d.Category).WithMany(p => p.Requests)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Request_Category");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.RequestCreateByNavigations)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Request_Account");

            entity.HasOne(d => d.ReceiveByNavigation).WithMany(p => p.RequestReceiveByNavigations)
                .HasForeignKey(d => d.ReceiveBy)
                .HasConstraintName("FK_Request_Account1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<StaffAccount>(entity =>
        {
            entity.ToTable("StaffAccount");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("date");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Fullname).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(30);
            entity.Property(e => e.Phone).HasMaxLength(12);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UpdateDate).HasColumnType("date");
            entity.Property(e => e.Username).HasMaxLength(30);

            entity.HasOne(d => d.Role).WithMany(p => p.StaffAccounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_StaffAccount_Role");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transaction");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.OrderId)
                .HasMaxLength(20)
                .HasColumnName("OrderID");
            entity.Property(e => e.OrderInfo).HasMaxLength(255);
            entity.Property(e => e.PaymentMethod).HasMaxLength(30);
            entity.Property(e => e.ResponseCode).HasMaxLength(5);
            entity.Property(e => e.VnPayId)
                .HasMaxLength(20)
                .HasColumnName("VnPayID");

            entity.HasOne(d => d.Account).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Transaction_Account");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
