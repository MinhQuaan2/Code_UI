using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class Role
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<StaffAccount> StaffAccounts { get; set; } = new List<StaffAccount>();
}
