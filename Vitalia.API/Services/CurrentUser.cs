using System.Security.Claims;
using Vitalia.Application.Interfaces.Services;

namespace Vitalia.API.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor)
    : ICurrentUser
{
    private ClaimsPrincipal User =>
        httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException(
            "Não foi possível identificar o usuário autenticado."
        );

    public long UserId
    {
        get
        {
            var userIdClaim = User
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

    public bool IsInRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return false;
        }

        return User
            .FindAll("roles")
            .Any(claim =>
                string.Equals(
                    claim.Value,
                    role,
                    StringComparison.OrdinalIgnoreCase));
    }
}