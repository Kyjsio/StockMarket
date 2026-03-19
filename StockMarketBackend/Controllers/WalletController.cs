using Microsoft.AspNetCore.Mvc;
using StockMarketBackend.ModelsDto;
using StockMarketBackend.Services;

[Route("api/[controller]")]
[ApiController]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly IUserContextService _userContextService;

    public WalletController(IWalletService walletService, IUserContextService userContextService)
    {
        _walletService = walletService;
        _userContextService = userContextService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPortfolio()
    {
        var result = await _walletService.GetPortfolioAsync(_userContextService.GetUserId());
        return Ok(result);
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] BalanceUpdateDto request)
    {
        if (request.Amount <= 0)
            return BadRequest(new { message = "Kwota musi być dodatnia." });

        await _walletService.DepositAsync(_userContextService.GetUserId(), request.Amount);
        return Ok(new { message = "Środki zostały dodane na koncie" });
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] BalanceUpdateDto request)
    {
        if (request.Amount <= 0)
            return BadRequest(new { message = "Kwota wypłaty musi być dodatnia" });

        var result = await _walletService.WithdrawAsync(_userContextService.GetUserId(), request.Amount);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<WalletHistoryDto>>> GetWalletHistory()
    {
        var history = await _walletService.GetHistoryAsync(_userContextService.GetUserId());
        return Ok(history);
    }
}