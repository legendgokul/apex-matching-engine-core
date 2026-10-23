using System;
using System.Collections.Generic;

namespace apexLedger.DataAccess.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string? CompanyName { get; set; }

    public string? CompanyUrl { get; set; }

    public DateTime? Createdon { get; set; }

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
