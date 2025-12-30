using Broker_Projekt_Zaliczeniowy.Models;
using Broker_Projekt_Zaliczeniowy.ModelsDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Broker_Projekt_Zaliczeniowy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ProjektBdContext _context;
        public AdminController(ProjektBdContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("add-asset")]
        public async Task<IActionResult> AddAsset([FromBody] AddAssetDto request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Nieprawidłowe dane formularza" });
            }
            if (await _context.Assets.AnyAsync(a => a.Ticker == request.Ticker))
            {
                return Conflict(new { message = $"Spólka o tickerze {request.Ticker} już istnieje w systemie" });
            }
            var newAsset = new Asset
            {
                Ticker = request.Ticker,
                FullName = request.FullName,
                Type = "Stock"
            };
            _context.Assets.Add(newAsset);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Sukces! Dodano spółkę {request.FullName}" });
        }
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UsersDataDto>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Account)
                .Where(u => u.Role == "User")
                .Select(u => new UsersDataDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt, 
                    Balance =  u.Account.Balance,
                    Role = u.Role
                })
                .ToListAsync();

            return Ok(users);
        }
        [HttpDelete("delete-user/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
    
            var user = await _context.Users
                .Include(u => u.Account)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "Użytkownik nie został znaleziony" });
            }

            if (user.Role == "Admin")
            {
                return BadRequest(new { message = "Nie można usunąć konta Administratora" });
            }


            _context.Users.Remove(user);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = $"Użytkownik {user.Email} został usunięty" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Nie można usunąć użytkownika.Prawdopodobnie posiada aktywne transakcje lub logi blokujące usunięcie", error = ex.Message });
            }
        }
        [HttpGet("reports/users")]
        public async Task<ActionResult> GetUsersReport()
        {
            try
            {
                var report = await _context.AdminUserReportResults
                    .FromSqlRaw("EXEC sp_GetAdminUserReport")
                    .ToListAsync();

                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Błąd generowania raportu użytkowników", error = ex.Message });
            }
        }

        [HttpGet("reports/stats")]
        public async Task<IActionResult> GetSystemStats()
        {
            try
            {
                var statsList = await _context.SystemStatsResults
                    .FromSqlRaw("EXEC sp_GetSystemStats")
                    .ToListAsync();

                var stats = statsList.FirstOrDefault();

                if (stats == null)
                    return NotFound(new { message = "Nie udało się obliczyć statystyk" });

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Błąd generowania statystyk systemu", error = ex.Message });
            }
        }
    }

}