using StockMarketBackend.ModelsDto;
using StockMarketBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StockMarketBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-asset")]
        public async Task<IActionResult> AddAsset([FromBody] AddAssetDto request)
        {
            if (request == null) return BadRequest(new { message = "Nieprawidłowe dane formularza" });

            var result = await _adminService.AddAssetAsync(request);

            if (!result.Success) return Conflict(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UsersDataDto>>> GetUsers()
        {
            var users = await _adminService.GetUsersAsync();
            return Ok(users);
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var result = await _adminService.DeleteUserAsync(id);

            if (!result.Success)
            {
                if (result.ErrorType == "NotFound") return NotFound(new { message = result.Message });
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        [HttpGet("reports/users")]
        public async Task<ActionResult> GetUsersReport()
        {
            try
            {
                var report = await _adminService.GetUsersReportAsync();
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
                var stats = await _adminService.GetSystemStatsAsync();
                if (stats == null) return NotFound(new { message = "Nie udało się obliczyć statystyk" });

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Błąd generowania statystyk systemu", error = ex.Message });
            }
        }
    }
}