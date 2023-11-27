using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseSplitter.Api.Application.Abstractions.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
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

        try
        {
            logger.LogInformation("Executing command {Command}", name);

            var result = await next();

            logger.LogInformation("Command {Command} processed successfully", name);

            return result;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Command {Command} processing failed", name);

            throw;
        }
    }
}