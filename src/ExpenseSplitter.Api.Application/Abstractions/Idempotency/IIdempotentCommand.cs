namespace ExpenseSplitter.Api.Application.Abstractions.Idempotency;

public interface IIdempotentCommand
{
    Guid RequestId { get; }
}