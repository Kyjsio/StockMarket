using Broker_Projekt_Zaliczeniowy.Models;
using Broker_Projekt_Zaliczeniowy.ModelsDto;
using Microsoft.EntityFrameworkCore;

namespace Broker_Projekt_Zaliczeniowy.Services
{
    public class GetPortfolioService
    {
        private readonly ProjektBdContext _context;

        public GetPortfolioService(ProjektBdContext context)
        {
            _context = context;
        }

        public async Task<PortfolioDto> GetUserPortfolioAsync(int userId)
        {
            var account = await _context.Accounts
                .Include(a => a.Positions)
                .ThenInclude(p => p.Asset)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
            {
                throw new KeyNotFoundException("Nie znaleziono konta maklerskiego dla tego użytkownika.");
            }

            var response = new PortfolioDto
            {
                CashBalance = account.Balance,
                Positions = new List<PortfolioPositionDto>()
            };

            foreach (var pos in account.Positions)
            {
                var lastMarketData = await _context.MarketData
                    .Where(m => m.AssetId == pos.AssetId)
                    .OrderByDescending(m => m.DataDate)
                    .FirstOrDefaultAsync();

                decimal currentPrice = lastMarketData?.Close ?? 0;

                var positionDto = new PortfolioPositionDto
                {
                    AssetId = pos.AssetId,
                    Ticker = pos.Asset.Ticker,
                    FullName = pos.Asset.FullName,
                    Quantity = pos.Quantity,
                    AverageCost = pos.AverageCost,
                    CurrentPrice = currentPrice,
                    CurrentValue = pos.Quantity * currentPrice,
                    ProfitLoss = (pos.Quantity * currentPrice) - (pos.Quantity * pos.AverageCost)
                };

                if (pos.AverageCost > 0)
                {
                    positionDto.ProfitLossPercentage = ((currentPrice - pos.AverageCost) / pos.AverageCost) * 100;
                }
                else
                {
                    positionDto.ProfitLossPercentage = 0;
                }

                response.Positions.Add(positionDto);
            }

            response.TotalAssetsValue = response.Positions.Sum(p => p.CurrentValue);
            response.TotalPortfolioValue = response.CashBalance + response.TotalAssetsValue;

            return response;
        }
    }
}