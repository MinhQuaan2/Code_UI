using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class DonationBenefit
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<DonationDetailBenefit> DonationDetailBenefits { get; set; } = new List<DonationDetailBenefit>();
}
