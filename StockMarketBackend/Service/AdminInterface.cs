using StockMarketBackend.ModelsDto;

namespace StockMarketBackend.Services
{
    public interface IAdminService
    {
        Task<(bool Success, string Message)> AddAssetAsync(AddAssetDto request);
        Task<IEnumerable<UsersDataDto>> GetUsersAsync();
        Task<(bool Success, string Message, string ErrorType)> DeleteUserAsync(int id);
        Task<IEnumerable<object>> GetUsersReportAsync();
        Task<object> GetSystemStatsAsync();
    }
}