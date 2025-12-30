namespace Broker_Projekt_Zaliczeniowy.ModelsDto
{
    public class TransactionRequestDto
    {
        public int AssetId { get; set; }
        public decimal Quantity { get; set; }
        public string Type { get; set; } = "BUY";

        public int BuyTransactionId { get; set; }
    }
    public class SellBatchDto
    {
        public int TransactionId { get; set; }
        public int AssetId { get; set; }
        public decimal Quantity { get; set; }
    }
        public class BalanceUpdateDto
    {
        public decimal Amount { get; set; }
    }
    public class PositionDetailsViewModel
    {
        public string Ticker { get; set; }
        public string CompanyName { get; set; }
        public int AssetId { get; set; }
        public decimal AverageCost { get; set; } 
        public decimal Profit { get; set; }     
        public decimal ProfitPercentage { get; set; }
        public decimal CurrentQuantity { get; set; }
        public decimal CurrentValue { get; set; }

        public List<TransactionDto> Transactions { get; set; }
    }
    public class TransactionDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } 
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitPercentage { get; set; }
        public bool CanSell { get; set; }   
    }
    public class StockTransactionDto
    {
        public int Id { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime Date { get; set; }
        public decimal Profit { get; set; }
    }
}