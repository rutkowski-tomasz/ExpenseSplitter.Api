using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Application.Abstractions.Idempotency;

internal sealed class IdempotentBehavior<TRequest, TResponse>(IIdempotencyService service)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!service.IsIdempotencyKeyInHeaders())
        {
            return await next();
        }

        if (!service.TryParseIdempotencyKey(out var parsedIdempotencyKey))
        {
            return CreateFailureResult(IdempotencyErrors.IdempotencyKeyIsNotGuid);
        }

        if (await service.IsIdempotencyKeyProcessed(parsedIdempotencyKey, cancellationToken))
        {
            return CreateFailureResult(IdempotencyErrors.IdempotentKeyAlreadyProcessed);
        }

        await service.SaveIdempotencyKey(
            parsedIdempotencyKey, 
            typeof(TRequest).Name,
            cancellationToken
        );

        var response = await next();

        return response;
    }
    
    private static TResponse CreateFailureResult(AppError appError)
    {
        if (!IsGenericResult())
        {
            return (TResponse) Result.Failure(appError);
        }

        var resultType = typeof(TResponse).GetGenericArguments()[0];
        var resultMethodInfos = typeof(Result).GetMethods();
        var failureMethod = resultMethodInfos.First(m => m is
        {
            IsGenericMethod: true,
            Name: nameof(Result.Failure)
        });
        var genericFailureMethod = failureMethod.MakeGenericMethod(resultType);
        var resultFailureObject = genericFailureMethod.Invoke(null, [appError]);

        return (TResponse) resultFailureObject!;
    }

    private static bool IsGenericResult()
    {
        return typeof(TResponse).IsGenericType
            && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>);
    }
}
