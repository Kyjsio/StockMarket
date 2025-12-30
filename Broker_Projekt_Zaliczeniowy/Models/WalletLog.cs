namespace Broker_Projekt_Zaliczeniowy.Models
{
   
    public class WalletLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal OldBalance { get; set; }
        public decimal NewBalance { get; set; }
        public DateTime ChangeDate { get; set; }
        public string ActionType { get; set; }
    }
}