using System;
using System.Collections.Generic;

namespace apexLedger.DataAccess.Models;

public partial class Userholding
{
    public int UserId { get; set; }

    public int StockId { get; set; }

    public int? Quantity { get; set; }

    public int? Boughtprice { get; set; }

    public bool? Isactive { get; set; }

    public virtual Stock Stock { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
