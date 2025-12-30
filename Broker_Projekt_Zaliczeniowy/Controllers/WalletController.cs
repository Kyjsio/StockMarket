using Broker_Projekt_Zaliczeniowy.Models;
using Broker_Projekt_Zaliczeniowy.ModelsDto;
using Broker_Projekt_Zaliczeniowy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Route("api/[controller]")]
[ApiController]
public class WalletController : ControllerBase
{
    private readonly ProjektBdContext _context;
    private readonly IUserContextService _userContextService;

    public WalletController(ProjektBdContext context, IUserContextService userContextService)
    {
        _context = context;
        _userContextService = userContextService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPortfolio()
    {
        int userId = _userContextService.GetUserId();
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.UserId == userId);

        decimal cashBalance = account.Balance;
        
        var positions = await _context.Positions
            .Where(p => p.AccountId == userId)
            .Select(p => new
            {
                p.AssetId,
                Ticker = p.Asset.Ticker,
                FullName = p.Asset.FullName,
                Quantity = p.Quantity,
                AverageCost = p.AverageCost,
                CurrentPrice = _context.MarketData
                                .Where(m => m.AssetId == p.AssetId)
                                .OrderByDescending(m => m.DataDate)
                                .Select(m => m.Close)
                                .FirstOrDefault()
            })
            .ToListAsync();

        var processedPositions = positions.Select(p => new
        {
            p.AssetId,
            p.Ticker,
            p.FullName,
            p.Quantity,
            AvgCost = p.AverageCost,
            CurrentPrice = p.CurrentPrice,
            CurrentValue = p.Quantity * p.CurrentPrice,
            ProfitLoss = (p.Quantity * p.CurrentPrice) - (p.Quantity * p.AverageCost),
            ProfitLossPercentage = p.AverageCost > 0
     
        }).ToList();
        decimal totalAssetsValue = processedPositions.Sum(p => p.CurrentValue);
        decimal totalPortfolioValue = totalAssetsValue + cashBalance;

        var result = new
        {
            CashBalance = cashBalance,
            TotalAssetsValue = totalAssetsValue,
            TotalPortfolioValue = totalPortfolioValue,
            Positions = processedPositions
        };

        return Ok(result);
    }


    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] BalanceUpdateDto request)
    {

        if (request.Amount <= 0)
            return BadRequest(new { message = "Kwota wypłaty musi być dodatnia." });
        int userId = _userContextService.GetUserId();

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
        if (account == null)
        {

            account = new Account { UserId = userId, Balance = 0 };
            _context.Accounts.Add(account);
        }

        decimal oldBalance = account.Balance;
        account.Balance += request.Amount;

        var log = new WalletLog
        {
            UserId = userId,
            OldBalance = oldBalance,
            NewBalance = account.Balance,
            ChangeDate = DateTime.Now,
            ActionType = "Deposit"
        };
        _context.WalletLogs.Add(log);

        await _context.SaveChangesAsync();
        return Ok(new { message = "Środki zostały dodane na koncie" });
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] BalanceUpdateDto request)
    {
        if (request.Amount <= 0)
            return BadRequest(new { message = "Kwota wypłaty musi być dodatnia" });
        int userId = _userContextService.GetUserId();

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
        if (account.Balance < request.Amount)
            return BadRequest(new { message = "Brak wystarczających środków na koncie do realizacji wypłaty" });


        decimal oldBalance = account.Balance;
        account.Balance -= request.Amount;

        var log = new WalletLog
        {
            UserId = userId,
            OldBalance = oldBalance,
            NewBalance = account.Balance,
            ChangeDate = DateTime.Now,
            ActionType = "Withdraw" 
        };
        _context.WalletLogs.Add(log);

        await _context.SaveChangesAsync();
        return Ok(new { message = "Wypłata zlecona pomyślnie" });
    }


    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<WalletHistoryDto>>> GetWalletHistory()
    {
        int userId = _userContextService.GetUserId();

        var logs = await _context.WalletLogs
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.ChangeDate)
            .ToListAsync();

        var history = logs.Select(l =>
        {
            decimal diff = Math.Abs(l.NewBalance - l.OldBalance);

            decimal finalAmount = diff;
            string title = "";
            string tag = "";

            string type = l.ActionType.ToUpper();

            if (type.Contains("DEPOSIT"))
            {
                finalAmount = diff; 
                title = "Zasilenie Konta";
                tag = "deposit";
            }
            else if (type.Contains("WITHDRAW"))
            {
                finalAmount = -diff; 
                title = "Wypłata Środków";
                tag = "withdraw";
            }
            else if (type.Contains("KUPNO"))
            {
                finalAmount = -diff; 
                title = "Zakup Akcji";
                tag = "buy";
            }
            else if (type.Contains("SPRZEDAŻ"))
            {
                finalAmount = diff; 
                title = "Sprzedaż Akcji";
                tag = "sell";
            }

            return new WalletHistoryDto
            {
                Id = l.Id,
                Date = l.ChangeDate,
                Amount = finalAmount,
                Title = title,
                OperationTag = tag
            };
        });

        return Ok(history);
    }
}