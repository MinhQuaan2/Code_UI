using System;
using System.Collections.Generic;

namespace CodeUI.Data.Entity;

public partial class Transaction
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string OrderInfo { get; set; } = null!;

    public string OrderId { get; set; } = null!;

    public string VnPayId { get; set; } = null!;

    public bool Status { get; set; }

    public Guid AccountId { get; set; }

    public long Amount { get; set; }

    public string ResponseCode { get; set; } = null!;

    public string? PaymentMethod { get; set; }

    public virtual Account Account { get; set; } = null!;
}
