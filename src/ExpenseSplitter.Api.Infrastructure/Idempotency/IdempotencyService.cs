using ExpenseSplitter.Api.Application.Abstractions.Idempotency;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Idempotency;

internal sealed class IdempotencyService : IIdempotencyService
{
    private readonly ApplicationDbContext context;

    public IdempotencyService(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task CreateRequest(
        Guid requestId,
        string name,
        CancellationToken cancellationToken
    )
    {
        var idempotentRequest = new IdempotentRequest()
        {
            Id = requestId,
            Name = name,
            CreatedOnUtc = DateTime.UtcNow,
        };

        await context.AddAsync(idempotentRequest, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RequestExists(
        Guid requestId,
        CancellationToken cancellationToken
    )
    {
        return await context
            .Set<IdempotentRequest>()
            .AnyAsync(x => x.Id == requestId, cancellationToken);
    }
}
