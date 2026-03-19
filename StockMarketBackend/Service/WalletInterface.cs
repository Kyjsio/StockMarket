using StockMarketBackend.ModelsDto;

public interface IWalletService
{
    Task<PortfolioDto> GetPortfolioAsync(int userId);
    Task<bool> DepositAsync(int userId, decimal amount);
    Task<(bool Success, string Message)> WithdrawAsync(int userId, decimal amount);
    Task<IEnumerable<WalletHistoryDto>> GetHistoryAsync(int userId);
}