using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Application.Abstractions.Idempotency;

public static class IdempotencyErrors
{
    public static readonly Error IdempotencyKeyIsNotGuid = new(
        ErrorType.PreConditionFailed,
        "The idempotency key from headers is not a valid guid"
    );
    
    public static readonly Error IdempotentKeyAlreadyProcessed = new(
        ErrorType.Conflict,
        "The idempotency key from headers was already processed"
    );
}
