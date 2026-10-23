using System;
using System.Collections.Generic;

namespace apexLedger.DataAccess.Models;

public partial class Sellledger
{
    public long LedgerId { get; set; }

    public int? StockId { get; set; }

    public int? UserId { get; set; }

    public int? Stockcount { get; set; }

    public int? Stockprice { get; set; }

    public DateTime? Createdon { get; set; }

    public virtual Stock? Stock { get; set; }

    public virtual User? User { get; set; }
}
