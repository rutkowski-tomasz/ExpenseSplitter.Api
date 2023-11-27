using System.Security.Claims;
using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace ExpenseSplitter.Api.Infrastructure.Authentication;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public string IdentityId =>
        httpContextAccessor
            .HttpContext?
            .User
            .Claims
            .SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?
            .Value ??
        throw new ApplicationException("User context is unavailable");
}