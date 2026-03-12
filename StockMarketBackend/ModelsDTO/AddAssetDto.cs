using Microsoft.AspNetCore.Mvc;

namespace StockMarketBackend.ModelsDto
{
    public class AddAssetDto 
    {
        public string Ticker { get; set; } = string.Empty;
         public string FullName { get; set; } = string.Empty;

    }
}
