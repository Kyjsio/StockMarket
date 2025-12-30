namespace Broker_Projekt_Zaliczeniowy.ModelsDto
{
    public class WalletHistoryDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }    
        public string Title { get; set; }        
        public string OperationTag { get; set; }
    }
}