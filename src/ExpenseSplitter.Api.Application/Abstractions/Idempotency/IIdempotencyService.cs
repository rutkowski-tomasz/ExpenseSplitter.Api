namespace ExpenseSplitter.Api.Application.Abstractions.Idempotency;

public interface IIdempotencyService
{
    Task<bool> RequestExists(Guid requestId, CancellationToken cancellationToken);

    Task CreateRequest(Guid requestId, string name, CancellationToken cancellationToken);
}

