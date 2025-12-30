using Broker_Projekt_Zaliczeniowy.ModelsDto;
using Broker_Projekt_Zaliczeniowy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace Broker_Projekt_Zaliczeniowy.Services
{
    public class AuthService
    {
        private readonly ProjektBdContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ProjektBdContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                throw new Exception("Ten adres email jest już zajęty");
            }
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var newUser = new User
            {
                Email = dto.Email,
                PasswordHash = passwordHash,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CreatedAt = DateTime.Now,
                Role = "User"
            };

          
            var newAccount = new Account
            {
                Balance = 0,
                User = newUser 
            };

            _context.Users.Add(newUser);
            _context.Accounts.Add(newAccount);

            await _context.SaveChangesAsync();

            return "Konto utworzone! Możesz się teraz zalogować.";
        
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                throw new Exception("Nieprawidłowy login lub hasło");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new Exception("Nieprawidłowy login lub hasło");
            }

            string token = CreateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                UserId = user.Id,
                Role = user.Role
            };
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("JwtSettings:Issuer").Value,
                audience: _configuration.GetSection("JwtSettings:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddDays(1), 
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}