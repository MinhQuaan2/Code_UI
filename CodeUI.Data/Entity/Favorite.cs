using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class Favorite
{
    public int Id { get; set; }

    public DateTime? Timestamp { get; set; }

    public int? ElementId { get; set; }

    public Guid? AccountId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Element? Element { get; set; }
}
