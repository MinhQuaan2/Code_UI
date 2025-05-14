using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class Profile
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Location { get; set; }

    public string? Description { get; set; }

    public decimal? Wallet { get; set; }

    public string? ImageUrl { get; set; }

    public virtual Account? Account { get; set; }
}
