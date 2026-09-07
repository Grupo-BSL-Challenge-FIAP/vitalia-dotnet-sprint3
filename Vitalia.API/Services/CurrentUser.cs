using Vitalia.Application.Interfaces.Services;

namespace Vitalia.API.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor)
    : ICurrentUser
{
    public long UserId
    {
        get
        {
            var userIdClaim = httpContextAccessor
                .HttpContext?
                .User
                .FindFirst("userId")?
                .Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
            {
                throw new UnauthorizedAccessException(
                    "O usuário autenticado não possui um userId válido."
                );
            }

            if (!long.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException(
                    "O userId do token JWT é inválido."
                );
            }

            return userId;
        }
    }
}