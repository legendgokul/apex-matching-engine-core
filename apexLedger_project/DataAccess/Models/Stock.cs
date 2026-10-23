using System;
using System.Collections.Generic;

namespace apexLedger.DataAccess.Models;

public partial class Stock
{
    public int StockId { get; set; }

    public string? StockName { get; set; }

    public string? StockSymbol { get; set; }

    public int? CompanyId { get; set; }

    public DateTime? Createdon { get; set; }

    public virtual ICollection<Buyledger> Buyledgers { get; set; } = new List<Buyledger>();

    public virtual Company? Company { get; set; }

    public virtual ICollection<Sellledger> Sellledgers { get; set; } = new List<Sellledger>();

    public virtual ICollection<Transactionrecord> Transactionrecords { get; set; } = new List<Transactionrecord>();

    public virtual ICollection<Userholding> Userholdings { get; set; } = new List<Userholding>();
}
