using Broker_Projekt_Zaliczeniowy.Services;
using Broker_Projekt_Zaliczeniowy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Projekt_Broker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketController : ControllerBase
    {
        private readonly ProjektBdContext  _context;
        private readonly MarketDataService _marketService;

        public MarketController(ProjektBdContext context, MarketDataService marketService)
        {
            _context = context;
            _marketService = marketService;
        }

        [HttpGet("assets")]
        public async Task<IActionResult> GetAssets()
        {
            var assets = await _context.Assets
                .Select(a => new { a.Id, a.Ticker, a.FullName })
                .ToListAsync();

            return Ok(assets);
        }


        [HttpGet("history/{assetId}")]
        public async Task<IActionResult> GetHistory(int assetId)
        {
            var data = await _context.MarketData
                .Where(m => m.AssetId == assetId)
                .OrderByDescending(m => m.DataDate)
                .Select(m => new
                { 
                    Date = m.DataDate.ToString("yyyy-MM-dd"),
                    Price = m.Close

                })
                .ToListAsync();

            return Ok(data);
        }
        [HttpPost("update/{ticker}")]
        public async Task<IActionResult> UpdatePrices(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                return BadRequest("Ticker nie może być pusty");
            }
            var resultMessage = await _marketService.UpdateMarketDataAsync(ticker.ToUpper());
            return Ok(new { message = "Sukces zaktualizowano akcje" });
        }
    }
}