using System;
using System.Collections.Generic;

namespace StockMarketBackend.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Role { get; set; }

    public virtual Account? Account { get; set; }
}
