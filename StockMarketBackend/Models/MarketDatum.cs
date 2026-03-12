using System;
using System.Collections.Generic;

namespace StockMarketBackend.Models;

public partial class MarketDatum
{
    public int Id { get; set; }

    public int AssetId { get; set; }

    public DateOnly DataDate { get; set; }

    public decimal? Open { get; set; }

    public decimal? High { get; set; }

    public decimal? Low { get; set; }

    public decimal Close { get; set; }

    public virtual Asset Asset { get; set; } = null!;
}
