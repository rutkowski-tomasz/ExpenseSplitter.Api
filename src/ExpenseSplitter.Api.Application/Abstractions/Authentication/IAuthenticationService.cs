namespace ExpenseSplitter.Api.Application.Abstractions.Authentication;

public interface IAuthenticationService
{
    Task<string> RegisterAsync(
        string email,
        string password,
        CancellationToken cancellationToken);
}
