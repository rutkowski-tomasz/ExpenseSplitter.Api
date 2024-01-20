using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Application.Abstractions.Idempotency;

internal sealed class IdempotentBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{
    private static readonly Error IdempotencyKeyIsNotGuid = new(
        "Idempotency.IdempotentKeyNotGuid",
        "The idempotency key from headers is not a valid guid"
    );
    
    private static readonly Error IdempotentKeyAlreadyProcessed = new(
        "Idempotency.IdempotentKeyAlreadyProcessed",
        "The idempotency key from headers was already processed"
    );

    private readonly IIdempotencyService idempotencyService;

    public IdempotentBehavior(IIdempotencyService idempotencyService)
    {
        this.idempotencyService = idempotencyService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!idempotencyService.IsIdempotencyKeyInHeaders())
        {
            return await next();
        }

        if (!idempotencyService.TryParseIdempotencyKey(out var parsedIdempotencyKey))
        {
            return CreateFailureResult(IdempotencyKeyIsNotGuid);
        }

        if (await idempotencyService.IsIdempotencyKeyProcessed(parsedIdempotencyKey, cancellationToken))
        {
            return CreateFailureResult(IdempotentKeyAlreadyProcessed);
        }

        await idempotencyService.SaveIdempotencyKey(
            parsedIdempotencyKey, 
            typeof(TRequest).Name,
            cancellationToken
        );

        var response = await next();

        return response;
    }
    
    private static TResponse CreateFailureResult(Error error)
    {
        if (!IsGenericResult())
        {
            return (TResponse) Result.Failure(error);
        }

        var resultType = typeof(TResponse).GetGenericArguments()[0];
        var resultMethodInfos = typeof(Result).GetMethods();
        var failureMethod = resultMethodInfos.First(m => m is
        {
            IsGenericMethod: true,
            Name: nameof(Result.Failure)
        });
        var genericFailureMethod = failureMethod.MakeGenericMethod(resultType);
        var resultFailureObject = genericFailureMethod.Invoke(null, new object[] { error });

        return (TResponse) resultFailureObject!;
    }

    private static bool IsGenericResult()
    {
        return typeof(TResponse).IsGenericType && 
               typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>);
    }
}