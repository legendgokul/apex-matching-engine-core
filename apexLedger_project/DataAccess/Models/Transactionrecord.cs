using System;
using System.Collections.Generic;

namespace apexLedger.DataAccess.Models;

public partial class Transactionrecord
{
    public long TradeId { get; set; }

    public int? BuyerId { get; set; }

    public int? SellerId { get; set; }

    public int? StockId { get; set; }

    public int? Saleprice { get; set; }

    public DateTime? Tnxtime { get; set; }

    public virtual User? Buyer { get; set; }

    public virtual User? Seller { get; set; }

    public virtual Stock? Stock { get; set; }
}
