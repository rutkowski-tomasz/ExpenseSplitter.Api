using System.Security.Claims;
using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Domain.Users;
using Microsoft.AspNetCore.Http;

namespace ExpenseSplitter.Api.Infrastructure.Authentication;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public UserId UserId
    {
        get
        {
            var claim = httpContextAccessor
                .HttpContext?
                .User
                .Claims
                .SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?
                .Value;
            
            if (claim is null)
            {
                throw new ApplicationException("User context is unavailable");
            }

            var guid = Guid.Parse(claim);
            var userId = new UserId(guid);
            return userId;
        }
    }
}
