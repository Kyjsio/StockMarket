using Broker_Projekt_Zaliczeniowy.Models;
using Broker_Projekt_Zaliczeniowy.ModelsDto;
using Microsoft.EntityFrameworkCore;

namespace Broker_Projekt_Zaliczeniowy.Services
{
    public class PortfolioService
    {
        private readonly ProjektBdContext _context;

        public PortfolioService(ProjektBdContext context)
        {
            _context = context;
        }


        public async Task<TransactionResultDto> BuyStockAsync(int userId, TransactionRequestDto request)
        {
            if (request.Quantity <= 0)
                throw new ArgumentException("Ilość musi być większa od zera.");

            var account = await _context.Accounts
                .Include(a => a.Positions)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
                throw new KeyNotFoundException("Nie znaleziono konta maklerskiego.");

            var latestData = await _context.MarketData
                .Where(m => m.AssetId == request.AssetId)
                .OrderByDescending(m => m.DataDate)
                .FirstOrDefaultAsync();

            if (latestData == null)
                throw new InvalidOperationException("Brak danych rynkowych. Zaktualizuj ceny");

            decimal currentPrice = latestData.Close;
            decimal totalValue = currentPrice * request.Quantity;

            if (account.Balance < totalValue)
                throw new InvalidOperationException("Niewystarczające środki");

            account.Balance -= totalValue;

            var position = account.Positions.FirstOrDefault(p => p.AssetId == request.AssetId);
            if (position == null)
            {
                position = new Position { AccountId = account.Id, AssetId = request.AssetId, Quantity = request.Quantity, AverageCost = currentPrice };
                _context.Positions.Add(position);
            }
            else
            {
                decimal totalCostOld = position.Quantity * position.AverageCost;
                decimal totalCostNew = request.Quantity * currentPrice;
                position.AverageCost = (totalCostOld + totalCostNew) / (position.Quantity + request.Quantity);
                position.Quantity += request.Quantity;
            }

            var transaction = new Transaction
            {
                AccountId = account.Id,
                AssetId = request.AssetId,
                Type = "BUY",
                Quantity = request.Quantity,
                Price = currentPrice,
                TotalAmount = totalValue,
                TransactionDate = DateTime.Now,
                Profit = 0
            };
            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return new TransactionResultDto
            {
                Message = "Zakup zakończony pomyślnie!",
                NewBalance = account.Balance
            };
        }

        // =================================================================
        // LOGIKA SELL (Skopiowana z Twojego Kontrolera)
        // =================================================================
        public async Task<TransactionResultDto> SellStockAsync(int userId, SellBatchDto request)
        {
            if (request.Quantity <= 0)
                throw new ArgumentException("Ilość musi być większa od zera.");

            var account = await _context.Accounts
                .Include(a => a.Positions)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
                throw new KeyNotFoundException("Nie znaleziono konta.");

            var specificBatch = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == request.TransactionId && t.AccountId == account.Id);

            // Zabezpieczenie, gdyby nie znaleziono partii (rzucamy wyjątek zamiast null)
            if (specificBatch == null)
                throw new KeyNotFoundException("Nie znaleziono wskazanej transakcji zakupu.");

            var latestData = await _context.MarketData
                .Where(m => m.AssetId == request.AssetId)
                .OrderByDescending(m => m.DataDate)
                .FirstOrDefaultAsync();

            // Zabezpieczenie na brak danych (tak jak w Buy)
            if (latestData == null)
                throw new InvalidOperationException("Brak danych rynkowych.");

            decimal currentPrice = latestData.Close;
            decimal totalValue = currentPrice * request.Quantity;

            var position = account.Positions.FirstOrDefault(p => p.AssetId == request.AssetId);

            if (position == null)
                throw new InvalidOperationException("Błąd spójności portfela.");

            decimal profit = (currentPrice - specificBatch.Price) * request.Quantity;

            account.Balance += totalValue;
            position.Quantity -= request.Quantity;

            if (position.Quantity <= 0.000001m)
                _context.Positions.Remove(position);

            specificBatch.Type = "BUY_SOLD";
            _context.Entry(specificBatch).State = EntityState.Modified;

            var transaction = new Transaction
            {
                AccountId = account.Id,
                AssetId = request.AssetId,
                Type = "SELL",
                Quantity = request.Quantity,
                Price = currentPrice,
                TotalAmount = totalValue,
                TransactionDate = DateTime.Now,
                Profit = profit
            };
            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return new TransactionResultDto
            {
                Message = "Partia sprzedana pomyślnie",
                NewBalance = account.Balance
            };
        }
    }
}