using System;
using System.Collections.Generic;

namespace StockMarketBackend.Models;

public partial class WalletLog
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public decimal? OldBalance { get; set; }

    public decimal? NewBalance { get; set; }

    public DateTime? ChangeDate { get; set; }

    public string? ActionType { get; set; }
}
