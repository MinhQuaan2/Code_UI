using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class DonationPackageDetail
{
    public int Id { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public virtual ICollection<DonationDetailBenefit> DonationDetailBenefits { get; set; } = new List<DonationDetailBenefit>();

    public virtual DonationPackage? DonationPackage { get; set; }
}
