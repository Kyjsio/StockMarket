using StockMarketBackend.Models;
using StockMarketBackend.ModelsDto;
using Microsoft.EntityFrameworkCore;

namespace StockMarketBackend.Services
{
    public class AdminService : IAdminService
    {
        private readonly ProjektBdContext _context;

        public AdminService(ProjektBdContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> AddAssetAsync(AddAssetDto request)
        {
            if (await _context.Assets.AnyAsync(a => a.Ticker == request.Ticker))
            {
                return (false, $"Spółka o tickerze {request.Ticker} już istnieje w systemie");
            }

            var newAsset = new Asset
            {
                Ticker = request.Ticker,
                FullName = request.FullName,
                Type = "Stock"
            };

            _context.Assets.Add(newAsset);
            await _context.SaveChangesAsync();

            return (true, $"Sukces! Dodano spółkę {request.FullName}");
        }

        public async Task<IEnumerable<UsersDataDto>> GetUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Account)
                .Where(u => u.Role == "User")
                .Select(u => new UsersDataDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt ?? DateTime.MinValue,
                    Balance = u.Account.Balance,
                    Role = u.Role
                })
                .ToListAsync();
        }

        public async Task<(bool Success, string Message, string ErrorType)> DeleteUserAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Account)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return (false, "Użytkownik nie został znaleziony", "NotFound");
            if (user.Role == "Admin") return (false, "Nie można usunąć konta Administratora", "BadRequest");

            _context.Users.Remove(user);

            try
            {
                await _context.SaveChangesAsync();
                return (true, $"Użytkownik {user.Email} został usunięty", string.Empty);
            }
            catch (Exception ex)
            {
                return (false, "Nie można usunąć użytkownika. Prawdopodobnie posiada aktywne transakcje.", "Exception");
            }
        }

        public async Task<IEnumerable<object>> GetUsersReportAsync()
        {
            return await _context.AdminUserReportResults
                .FromSqlRaw("EXEC sp_GetAdminUserReport")
                .ToListAsync();
        }

        public async Task<object> GetSystemStatsAsync()
        {
            var statsList = await _context.SystemStatsResults
                .FromSqlRaw("EXEC sp_GetSystemStats")
                .ToListAsync();

            return statsList.FirstOrDefault();
        }
    }
}