namespace Broker_Projekt_Zaliczeniowy.Models;

public partial class Transaction
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public int AssetId { get; set; }
    public string Type { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }

    public decimal TotalAmount { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Profit { get; set; } 

    public virtual Account Account { get; set; } = null!;
    public virtual Asset Asset { get; set; } = null!;
}