using System.Security.Claims;

namespace Broker_Projekt_Zaliczeniowy.Services
{
    public interface IUserContextService
    {
        int GetUserId();
    }
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId()
        {
            var user = _httpContextAccessor.HttpContext.User;

            if (user == null)
            {
                throw new InvalidOperationException("Kontekst użytkownika jest niedostępny.");
            }
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (idClaim == null)
            {
                throw new UnauthorizedAccessException("Użytkownik nie jest zalogowany lub token jest nieprawidłowy (brak ID).");
            }

            if (int.TryParse(idClaim.Value, out int userId))
            {
                return userId;
            }

            throw new Exception("Błąd parsowania ID użytkownika z tokena.");
        }
    }
}