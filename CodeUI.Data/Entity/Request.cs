using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class Request
{
    public int Id { get; set; }

    public string RequestDescription { get; set; } = null!;

    public string Status { get; set; } = null!;

    public decimal Reward { get; set; }

    public Guid CreateBy { get; set; }

    public Guid? ReceiveBy { get; set; }

    public string Name { get; set; } = null!;

    public decimal Deposit { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime StartDate { get; set; }

    public int Deadline { get; set; }

    public string? ImageUrl1 { get; set; }

    public string? ImageUrl2 { get; set; }

    public string? ImageUrl3 { get; set; }

    public int CategoryId { get; set; }

    public string Avatar { get; set; } = null!;

    public string? TypeCss { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Account CreateByNavigation { get; set; } = null!;

    public virtual ICollection<Fulfillment> Fulfillments { get; set; } = new List<Fulfillment>();

    public virtual Account? ReceiveByNavigation { get; set; }
}
