using StockMarketBackend.Models;
using StockMarketBackend.ModelsDto;
using Microsoft.EntityFrameworkCore;


public class WalletService : IWalletService
{
    private readonly ProjektBdContext _context;

    public WalletService(ProjektBdContext context)
    {
        _context = context;
    }

    public async Task<PortfolioDto> GetPortfolioAsync(int userId)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
        decimal cashBalance = account?.Balance ?? 0;

        var positionsData = await _context.Positions
            .Where(p => p.AccountId == userId)
            .Select(p => new
            {
                p.AssetId,
                Ticker = p.Asset.Ticker,
                FullName = p.Asset.FullName,
                p.Quantity,
                p.AverageCost,
                CurrentPrice = _context.MarketData
                                .Where(m => m.AssetId == p.AssetId)
                                .OrderByDescending(m => m.DataDate)
                                .Select(m => m.Close)
                                .FirstOrDefault()
            })
            .ToListAsync();

        var processedPositions = positionsData.Select(p => new PortfolioPositionDto
        {
            AssetId = p.AssetId,
            Ticker = p.Ticker,
            FullName = p.FullName,
            Quantity = p.Quantity,
            AverageCost = p.AverageCost,
            CurrentPrice = p.CurrentPrice,
            CurrentValue = p.Quantity * p.CurrentPrice,
            ProfitLoss = (p.Quantity * p.CurrentPrice) - (p.Quantity * p.AverageCost),
            ProfitLossPercentage = p.AverageCost > 0 ? ((p.CurrentPrice - p.AverageCost) / p.AverageCost) * 100 : 0
        }).ToList();

        decimal totalAssetsValue = processedPositions.Sum(p => p.CurrentValue);

        return new PortfolioDto
        {
            CashBalance = cashBalance,
            TotalAssetsValue = totalAssetsValue,
            TotalPortfolioValue = totalAssetsValue + cashBalance,
            Positions = processedPositions
        };
    }

    public async Task<bool> DepositAsync(int userId, decimal amount)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
        if (account == null)
        {
            account = new Account { UserId = userId, Balance = 0 };
            _context.Accounts.Add(account);
        }

        decimal oldBalance = account.Balance;
        account.Balance += amount;

        _context.WalletLogs.Add(new WalletLog
        {
            UserId = userId,
            OldBalance = oldBalance,
            NewBalance = account.Balance,
            ChangeDate = DateTime.Now,
            ActionType = "Deposit"
        });

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<(bool Success, string Message)> WithdrawAsync(int userId, decimal amount)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
        if (account == null || account.Balance < amount)
            return (false, "Brak wystarczających środków na koncie.");

        decimal oldBalance = account.Balance;
        account.Balance -= amount;

        _context.WalletLogs.Add(new WalletLog
        {
            UserId = userId,
            OldBalance = oldBalance,
            NewBalance = account.Balance,
            ChangeDate = DateTime.Now,
            ActionType = "Withdraw"
        });

        await _context.SaveChangesAsync();
        return (true, "Wypłata zlecona pomyślnie.");
    }

    public async Task<IEnumerable<WalletHistoryDto>> GetHistoryAsync(int userId)
    {
        var logs = await _context.WalletLogs
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.ChangeDate)
            .ToListAsync();

        return logs.Select(l =>
        {
            decimal diff = Math.Abs(l.NewBalance - l.OldBalance);
            string type = l.ActionType.ToUpper();

            var history = new WalletHistoryDto { Id = l.Id, Date = l.ChangeDate };

            if (type.Contains("DEPOSIT")) { history.Amount = diff; history.Title = "Zasilenie Konta"; history.OperationTag = "deposit"; }
            else if (type.Contains("WITHDRAW")) { history.Amount = -diff; history.Title = "Wypłata Środków"; history.OperationTag = "withdraw"; }
            else if (type.Contains("KUPNO")) { history.Amount = -diff; history.Title = "Zakup Akcji"; history.OperationTag = "buy"; }
            else if (type.Contains("SPRZEDAŻ")) { history.Amount = diff; history.Title = "Sprzedaż Akcji"; history.OperationTag = "sell"; }

            return history;
        });
    }
}