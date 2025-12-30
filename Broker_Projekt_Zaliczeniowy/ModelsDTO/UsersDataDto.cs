using System.ComponentModel.DataAnnotations;

namespace Broker_Projekt_Zaliczeniowy.ModelsDto
{
    public class UsersDataDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } 
        public decimal Balance { get; set; }
        public string Role { get; set; }
    }
}