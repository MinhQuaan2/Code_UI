using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class Payment
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Provider { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
