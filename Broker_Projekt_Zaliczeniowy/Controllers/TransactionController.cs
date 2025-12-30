using Broker_Projekt_Zaliczeniowy.Models;
using Broker_Projekt_Zaliczeniowy.ModelsDto;
using Broker_Projekt_Zaliczeniowy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Broker_Projekt_Zaliczeniowy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ProjektBdContext _context;
        private readonly IUserContextService _userContextService;

        private readonly PortfolioService _portfolioService; 
        private readonly PositionService _positionService;   

        public TransactionController(ProjektBdContext context,PortfolioService portfolioService,PositionService positionService,IUserContextService userContextService) 
        {
            _context = context;
            _portfolioService = portfolioService; 
            _positionService = positionService;   
            _userContextService = userContextService;
        }

        [HttpPost("BuyStock")]
            public async Task<IActionResult> BuyStock([FromBody] TransactionRequestDto request)
            {
                try
                {
                    int userId = _userContextService.GetUserId();

                    var result = await _portfolioService.BuyStockAsync(userId, request);

                    return Ok(result);
                }
                catch (ArgumentException ex) 
                {
                    return BadRequest(new { message = ex.Message });
                }
                catch (KeyNotFoundException ex) 
                {
                    return NotFound(new { message = ex.Message });
                }
                catch (InvalidOperationException ex) 
                {
                    return BadRequest(new { message = ex.Message });
                }
                catch (Exception ex) 
                {
                    return StatusCode(500, new { message = "Wystąpił błąd serwera.", details = ex.Message });
                }
            }

            [HttpPost("SellStock")]
            public async Task<IActionResult> SellStock([FromBody] SellBatchDto request)
            {
                try
                {
                    int userId = _userContextService.GetUserId();

                    var result = await _portfolioService.SellStockAsync(userId, request);

                    return Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(new { message = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Wystąpił błąd serwera.", details = ex.Message });
                }
            }



    [HttpGet("GetClosedHistory")]
        public async Task<ActionResult<IEnumerable<StockTransactionDto>>> GetClosedHistory()
        {
            int userId = _userContextService.GetUserId();
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
            if (account == null) return NotFound(new { message = "Brak konta" });

            var transactions = await _context.Transactions
                .Where(t => t.AccountId == account.Id && t.Type == "SELL")
                .Include(t => t.Asset)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new StockTransactionDto
                {
                    Id = t.Id,
                    Ticker = t.Asset.Ticker,
                    CompanyName = t.Asset.FullName,
                    Type = t.Type,
                    Quantity = t.Quantity,
                    Price = t.Price,
                    TotalAmount = t.TotalAmount,
                    Date = t.TransactionDate,
                    Profit = t.Profit
                })
                .ToListAsync();

            return Ok(transactions);
        }


        [HttpGet("details/{ticker}")]
        public async Task<ActionResult<PositionDetailsViewModel>> GetPositionDetails(string ticker)
        {
            try
            {
                int userId = _userContextService.GetUserId();
                var result = await _positionService.GetPositionDetailsAsync(userId, ticker);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }
    }
}