using Broker_Projekt_Zaliczeniowy.ModelsDto;

namespace Broker_Projekt_Zaliczeniowy.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<StockTransactionDto>> GetClosedHistoryAsync(int userId);
    }
}