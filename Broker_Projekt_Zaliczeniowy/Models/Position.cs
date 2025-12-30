using System;
using System.Collections.Generic;

namespace Broker_Projekt_Zaliczeniowy.Models;

public partial class Position
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int AssetId { get; set; }

    public decimal Quantity { get; set; }

    public decimal AverageCost { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Asset Asset { get; set; } = null!;
}
