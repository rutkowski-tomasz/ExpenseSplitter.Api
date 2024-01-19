using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Application.Abstractions.Idempotency;

internal sealed class IdempotentCommandBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdempotentCommand
    where TResponse : Result
{
    public static readonly Error IdempotentError = new(
        "Idempotency.RequestIdAlreadyProcessed",
        "The request with given RequestId was already processed"
    );

    private readonly IIdempotencyService idempotencyService;

    public IdempotentCommandBehavior(IIdempotencyService idempotencyService)
    {
        this.idempotencyService = idempotencyService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (await idempotencyService.RequestExists(request.RequestId, cancellationToken))
        {
            return CreateFailureResult();
        }

        await idempotencyService.CreateRequest(
            request.RequestId,
            typeof(TRequest).Name,
            cancellationToken
        );

        var response = await next();

        return response;
    }
    
    private static TResponse CreateFailureResult()
    {
        if (!IsGenericResult())
        {
            return (TResponse) Result.Failure(IdempotentError);
        }

        var resultType = typeof(TResponse).GetGenericArguments()[0];
        var resultMethods = typeof(Result).GetMethods();
        var failureMethod = resultMethods.First(m => m is { IsGenericMethod: true, Name: nameof(Result.Failure) });
        var genericFailureMethod = failureMethod.MakeGenericMethod(resultType);
        var resultFailureObject = genericFailureMethod.Invoke(null, new object[] { IdempotentError });

        return (TResponse) resultFailureObject!;
    }

    private static bool IsGenericResult()
    {
        return typeof(TResponse).IsGenericType && 
               typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>);
    }
}