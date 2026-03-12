using StockMarketBackend.ModelsDto;
using StockMarketBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StockMarketBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly IUserContextService _userContextService;
        private readonly PortfolioService _portfolioService;
        private readonly PositionService _positionService;
        private readonly ITransactionService _transactionService; // Added

        public TransactionController(
            PortfolioService portfolioService,
            PositionService positionService,
            IUserContextService userContextService,
            ITransactionService transactionService) // Injected
        {
            _portfolioService = portfolioService;
            _positionService = positionService;
            _userContextService = userContextService;
            _transactionService = transactionService;
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
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Nieznalezione akcji" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd serwera.", details = ex.Message });
            }
        }

        [HttpGet("GetClosedHistory")]
        public async Task<ActionResult<IEnumerable<StockTransactionDto>>> GetClosedHistory()
        {
            try
            {
                int userId = _userContextService.GetUserId();
                var transactions = await _transactionService.GetClosedHistoryAsync(userId);
                return Ok(transactions);
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