using Microsoft.AspNetCore.Mvc;

namespace Broker_Projekt_Zaliczeniowy.ModelsDto
{
    public class AdminUserReportResult
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Balance { get; set; } 
        public decimal TotalRealizedProfit { get; set; } 
        public int TransactionsCount { get; set; }
    }


    public class SystemStatsResult
    {
        public int TotalUsers { get; set; }
        public decimal? TotalSystemCash { get; set; }
        public int TotalTradesExecuted { get; set; }
    }
}
