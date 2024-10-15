using ExpenseSplitter.Api.Application.Abstractions.Idempotency;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Idempotency;

internal sealed class IdempotencyService(
    ApplicationDbContext context,
    IHttpContextAccessor accessor
) : IIdempotencyService
{
    private const string IdempotencyKeyHeaderName = "X-Idempotency-Key";

    public bool IsIdempotencyKeyInHeaders()
    {
        var idempotencyKey = GetIdempotencyKeyFromHeaders();
        return idempotencyKey is not null;
    }

    public bool TryParseIdempotencyKey(out Guid parsedIdempotencyKey)
    {
        var idempotencyKey = GetIdempotencyKeyFromHeaders();
        var isParsed = Guid.TryParse(idempotencyKey, out parsedIdempotencyKey);
        return isParsed;
    }

    public async Task<bool> IsIdempotencyKeyProcessed(
        Guid parsedIdempotencyKey,
        CancellationToken cancellationToken
    )
    {
        var isProcessed = await context
            .Set<IdempotentRequest>()
            .AnyAsync(x => x.Id == parsedIdempotencyKey, cancellationToken);

        return isProcessed;
    }

    public async Task SaveIdempotencyKey(Guid parsedIdempotencyKey, string name, CancellationToken cancellationToken)
    {
        var idempotentRequest = new IdempotentRequest
        {
            Id = parsedIdempotencyKey,
            Name = name,
            CreatedOnUtc = DateTime.UtcNow
        };

        await context.AddAsync(idempotentRequest, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private string? GetIdempotencyKeyFromHeaders()
    {
        var headers = accessor.HttpContext?.Request.Headers;
        var idempotencyKey = headers![IdempotencyKeyHeaderName].FirstOrDefault();
        return idempotencyKey;
    }
}
