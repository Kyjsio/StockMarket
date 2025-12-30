using Broker_Projekt_Zaliczeniowy.Models;
using Broker_Projekt_Zaliczeniowy.ModelsDto;
using Microsoft.EntityFrameworkCore;

namespace Broker_Projekt_Zaliczeniowy.Services
{
    public class PositionService
    {
        private readonly ProjektBdContext _context;

        public PositionService(ProjektBdContext context)
        {
            _context = context;
        }

        public async Task<PositionDetailsViewModel> GetPositionDetailsAsync(int userId, string ticker)
        {
            var account = await _context.Accounts
                .Include(a => a.Positions)
                .ThenInclude(p => p.Asset)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
            {
                throw new KeyNotFoundException("Nie znaleziono konta maklerskiego.");
            }

            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Ticker == ticker);
            if (asset == null)
            {
                throw new KeyNotFoundException($"Nie znaleziono waloru: {ticker}");
            }

            var latestMarketPrice = await _context.MarketData
                .Where(m => m.AssetId == asset.Id)
                .OrderByDescending(m => m.DataDate)
                .Select(m => m.Close)
                .FirstOrDefaultAsync();

            var currentPosition = account.Positions.FirstOrDefault(p => p.AssetId == asset.Id);
            decimal currentPrice = latestMarketPrice > 0 ? latestMarketPrice : (currentPosition?.AverageCost ?? 0);

            var dbTransactions = await _context.Transactions
                .Where(t => t.AccountId == account.Id
                            && t.AssetId == asset.Id
                            && t.Type == "BUY")
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            var transactionDtos = dbTransactions.Select(t =>
            {
                decimal currentBatchValue = t.Quantity * currentPrice;
                decimal costBasis = t.TotalAmount;
                decimal rowProfit = currentBatchValue - costBasis;
                decimal rowProfitPct = costBasis != 0 ? (rowProfit / costBasis) * 100 : 0;

                return new TransactionDto
                {
                    Id = t.Id,
                    Date = t.TransactionDate,
                    Type = t.Type,
                    Quantity = t.Quantity,
                    Price = t.Price,
                    TotalAmount = costBasis,
                    Profit = rowProfit,
                    ProfitPercentage = rowProfitPct,
                    CanSell = true
                };
            }).ToList();

            decimal totalQuantity = transactionDtos.Sum(t => t.Quantity);
            decimal totalInvested = transactionDtos.Sum(t => t.TotalAmount);
            decimal currentMarketValue = totalQuantity * currentPrice;
            decimal weightedAverageCost = totalQuantity > 0 ? totalInvested / totalQuantity : 0;
            decimal totalProfit = currentMarketValue - totalInvested;
            decimal totalProfitPct = totalInvested > 0 ? (totalProfit / totalInvested) * 100 : 0;

            return new PositionDetailsViewModel
            {
                Ticker = asset.Ticker,
                CompanyName = asset.FullName,
                AssetId = asset.Id,
                CurrentQuantity = totalQuantity,
                CurrentValue = currentMarketValue,
                AverageCost = weightedAverageCost,
                Profit = totalProfit,
                ProfitPercentage = totalProfitPct,
                Transactions = transactionDtos
            };
        }
    }
}