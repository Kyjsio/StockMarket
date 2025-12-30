using System.Collections.Generic;
namespace Broker_Projekt_Zaliczeniowy.ModelsDto
{
    public class PortfolioDto
    {
        public decimal CashBalance { get; set; } 
        public decimal TotalAssetsValue { get; set; } 
        public decimal TotalPortfolioValue { get; set; } 
        public List<PortfolioPositionDto> Positions { get; set; } = new List<PortfolioPositionDto>();
    }

    public class PortfolioPositionDto
    {
        public int AssetId { get; set; }
        public string Ticker { get; set; }
        public string FullName { get; set; }
        public decimal Quantity { get; set; }
        public decimal AverageCost { get; set; } 
        public decimal CurrentPrice { get; set; } 
        public decimal CurrentValue { get; set; } 
        public decimal ProfitLoss { get; set; } 
        public decimal ProfitLossPercentage { get; set; } 
    }
}