using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseSplitter.Api.Application.Abstractions.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest
    where TResponse : Result
{
    private readonly ILogger<TRequest> logger;

    public LoggingBehavior(ILogger<TRequest> logger)
    {
        this.logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var name = request.GetType().Name;

        logger.LogInformation("Processing request {RequestName}", name);

        var result = await next();

        if (result.IsFailure)
        {
            logger.LogError(
                "Request failure {@RequestName}, {@Error}, {@DateTimeUtc}",
                typeof(TRequest).Name,
                result.Error,
                DateTime.UtcNow
            );
        }
        else
        {
            logger.LogInformation(
                "Request success {@RequestName}, {@DateTimeUtc}",
                typeof(TRequest).Name,
                DateTime.UtcNow
            );
        }
        
        return result;
    }
}