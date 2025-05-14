using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class StaffAccount
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public bool IsActive { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Fullname { get; set; }

    public virtual Role Role { get; set; } = null!;
}
