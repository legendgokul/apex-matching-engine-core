using System;
using System.Collections.Generic;

namespace apexLedger.DataAccess.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public DateTime? Createdon { get; set; }

    public virtual ICollection<Buyledger> Buyledgers { get; set; } = new List<Buyledger>();

    public virtual ICollection<Sellledger> Sellledgers { get; set; } = new List<Sellledger>();

    public virtual ICollection<Transactionrecord> TransactionrecordBuyers { get; set; } = new List<Transactionrecord>();

    public virtual ICollection<Transactionrecord> TransactionrecordSellers { get; set; } = new List<Transactionrecord>();

    public virtual ICollection<Userholding> Userholdings { get; set; } = new List<Userholding>();
}
