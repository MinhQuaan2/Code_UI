using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class DonationDetailBenefit
{
    public int Id { get; set; }

    public int DonationDetailId { get; set; }

    public int DonationBenefitId { get; set; }

    public string? Config { get; set; }

    public int? Number { get; set; }

    public string? Unit { get; set; }

    public virtual DonationBenefit DonationBenefit { get; set; } = null!;

    public virtual DonationPackageDetail DonationDetail { get; set; } = null!;
}
