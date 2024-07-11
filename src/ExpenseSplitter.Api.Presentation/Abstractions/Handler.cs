using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.Abstractions;

public interface IHandler<TRequest, TCommand, TCommandResult, TResponse>
    where TCommand : IRequest<Result<TCommandResult>>
{
    Task<IResult> Handle(TRequest request);
}

public interface IHandlerEmptyRequest<TCommand, TCommandResult, TResponse>
    where TCommand : IRequest<Result<TCommandResult>>, new()
{
    Task<IResult> Handle();
}

public interface IHandlerEmptyResponse<TRequest, TCommand>
    where TCommand : IRequest<Result>
{
    Task<IResult> Handle(TRequest request);
}

public abstract class BaseHandler(
    IHttpContextAccessor httpContextAccessor,
    ISender sender
)
{
    protected Task<Result<TCommandResult>> HandleCommand<TCommand, TCommandResult>()
        where TCommand : IRequest<Result<TCommandResult>>, new()
        => SendCommand<TCommand, Result<TCommandResult>>(new TCommand());

    protected Task<Result> HandleCommand<TRequest, TCommand>(TRequest request, IMapper<TRequest, TCommand> requestMapper)
        where TCommand : IRequest<Result>
        => SendCommand<TCommand, Result>(requestMapper.Map(request));

    protected Task<Result<TCommandResult>> HandleCommand<TRequest, TCommand, TCommandResult>(TRequest request, IMapper<TRequest, TCommand> requestMapper)
        where TCommand : IRequest<Result<TCommandResult>>
        => SendCommand<TCommand, Result<TCommandResult>>(requestMapper.Map(request));

    private async Task<TCommandResult> SendCommand<TCommand, TCommandResult>(TCommand command)
        where TCommand : IRequest<TCommandResult>
    {
        var cancellationToken = httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;
        var result = await sender.Send(command, cancellationToken);
        return result;
    }

    protected IResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok();
        }

        return HandleError(result.Error);
    }

    protected IResult HandleResult<TResult, TResponse>(Result<TResult> result, IMapper<TResult, TResponse> responseMapper)
    {
        if (result.IsSuccess)
        {
            var response = responseMapper.Map(result.Value);
            return TypedResults.Ok(response);
        }

        return HandleError(result.Error);
    }

    protected IResult HandleError(Error error)
    {
        var detail = error.Description;
        return error.Type switch
        {
            ErrorType.Validation => Results.BadRequest(detail),
            ErrorType.NotFound => Results.NotFound(detail),
            ErrorType.Forbidden => Results.Forbid(),
            ErrorType.BadRequest => Results.BadRequest(detail),
            ErrorType.PreConditionFailed => Results.Problem(detail, statusCode: StatusCodes.Status412PreconditionFailed),
            ErrorType.Conflict => Results.Conflict(detail),
            ErrorType.BadGateway => Results.Problem(detail, statusCode: StatusCodes.Status502BadGateway),
            ErrorType.Unauthorized => Results.Unauthorized(),
            ErrorType.NotModified => Results.StatusCode(StatusCodes.Status304NotModified),
            _ => Results.Problem(detail, statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}

public class Handler<TRequest, TCommand, TCommandResult, TResponse>(
    IHttpContextAccessor httpContextAccessor,
    ISender sender,
    IMapper<TRequest, TCommand> requestMapper,
    IMapper<TCommandResult, TResponse> responseMapper
) : BaseHandler(httpContextAccessor, sender), IHandler<TRequest, TCommand, TCommandResult, TResponse>
where TCommand : IRequest<Result<TCommandResult>>
{
    public async Task<IResult> Handle(TRequest request)
        => HandleResult(await HandleCommand<TRequest, TCommand, TCommandResult>(request, requestMapper), responseMapper);
}

public class HandlerEmptyResponse<TRequest, TCommand>(
    IHttpContextAccessor httpContextAccessor,
    ISender sender,
    IMapper<TRequest, TCommand> requestMapper
) : BaseHandler(httpContextAccessor, sender), IHandlerEmptyResponse<TRequest, TCommand>
where TCommand : IRequest<Result>
{
    public async Task<IResult> Handle(TRequest request)
        => HandleResult(await HandleCommand(request, requestMapper));
}

public class HandlerEmptyRequest<TCommand, TCommandResult, TResponse>(
    IHttpContextAccessor httpContextAccessor,
    ISender sender,
    IMapper<TCommandResult, TResponse> responseMapper
) : BaseHandler(httpContextAccessor, sender), IHandlerEmptyRequest<TCommand, TCommandResult, TResponse>
where TCommand : IRequest<Result<TCommandResult>>, new()
{
    public async Task<IResult> Handle()
        => HandleResult(await HandleCommand<TCommand, TCommandResult>(), responseMapper);
}
