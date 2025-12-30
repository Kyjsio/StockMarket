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
    public class PortfolioController : ControllerBase
    {
        private readonly ProjektBdContext _context;
        private readonly PortfolioService _portfolioService;
        private readonly IUserContextService _userContextService;

        public PortfolioController(ProjektBdContext context, IUserContextService userContextService, PortfolioService portfolioService)
        {
            _context = context;
            _userContextService = userContextService;
            _portfolioService = portfolioService;
        }

        [HttpGet]
        public async Task<ActionResult<PortfolioDto>> GetPortfolio()
        {
            try
            {
                int userId = _userContextService.GetUserId();

                var portfolio = await _portfolioService.GetUserPortfolioAsync(userId);

                return Ok(portfolio);
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
    }
}
