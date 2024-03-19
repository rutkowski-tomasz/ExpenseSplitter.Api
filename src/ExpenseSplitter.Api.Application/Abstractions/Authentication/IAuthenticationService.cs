using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Application.Abstractions.Authentication;

public interface IAuthenticationService
{
    Task<Result<string>> RegisterAsync(
        string email,
        string password,
        CancellationToken cancellationToken);
}
