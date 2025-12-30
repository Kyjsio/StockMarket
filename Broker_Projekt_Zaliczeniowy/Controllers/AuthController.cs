using Broker_Projekt_Zaliczeniowy.ModelsDto;
using Broker_Projekt_Zaliczeniowy.Services;
using Microsoft.AspNetCore.Mvc;

namespace Broker_Projekt_Zaliczeniowy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);

                
                return Ok(new
                {
                    message = "Zalogowano pomyślnie",
                    token = response.Token,
                    email = response.Email,
                    userId = response.UserId,
                    role = response.Role
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
