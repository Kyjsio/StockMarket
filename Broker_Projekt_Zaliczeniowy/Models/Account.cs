using System;
using System.Collections.Generic;

namespace Broker_Projekt_Zaliczeniowy.Models;

public partial class Account
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal Balance { get; set; }

    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;
}
