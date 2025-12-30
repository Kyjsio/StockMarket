using System;
using System.Collections.Generic;

namespace Broker_Projekt_Zaliczeniowy.Models;

public partial class Asset
{
    public int Id { get; set; }

    public string Ticker { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual ICollection<MarketDatum> MarketData { get; set; } = new List<MarketDatum>();

    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
