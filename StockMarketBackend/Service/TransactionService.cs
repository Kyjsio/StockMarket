using StockMarketBackend.Models;
using StockMarketBackend.ModelsDto;
using Microsoft.EntityFrameworkCore;

namespace StockMarketBackend.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ProjektBdContext _context;

        public TransactionService(ProjektBdContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockTransactionDto>> GetClosedHistoryAsync(int userId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
            {
                throw new KeyNotFoundException("Brak konta dla podanego użytkownika.");
            }

            return await _context.Transactions
                .Where(t => t.AccountId == account.Id && t.Type == "SELL")
                .Include(t => t.Asset)
                .OrderByDescending(t => t.TransactionDate)
               .Select(t => new StockTransactionDto
               {
                   Id = t.Id,
                   Ticker = t.Asset != null ? t.Asset.Ticker : "Brak",
                   CompanyName = t.Asset != null ? t.Asset.FullName : "Brak",
                   Type = t.Type,
                   Quantity = t.Quantity, 
                   Price = t.Price,
                   TotalAmount = t.TotalAmount, 

                   Date = t.TransactionDate ?? DateTime.MinValue,
                   Profit = t.Profit ?? 0m
               })
                .ToListAsync();
        }
    }
}