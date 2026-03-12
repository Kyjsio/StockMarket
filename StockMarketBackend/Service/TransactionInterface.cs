using StockMarketBackend.ModelsDto;

namespace StockMarketBackend.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<StockTransactionDto>> GetClosedHistoryAsync(int userId);
    }
}