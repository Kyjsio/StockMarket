namespace Broker_Projekt_Zaliczeniowy.ModelsDto
{ 
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Role { get; set; }
    }
}