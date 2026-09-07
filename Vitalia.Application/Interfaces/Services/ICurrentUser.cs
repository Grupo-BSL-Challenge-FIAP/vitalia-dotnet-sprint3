namespace Vitalia.Application.Interfaces.Services;

public interface ICurrentUser
{
    long UserId { get; }

    bool IsInRole(string role);
}