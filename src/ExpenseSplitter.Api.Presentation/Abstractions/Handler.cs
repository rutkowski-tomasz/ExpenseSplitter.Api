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

public class Handler<TRequest, TCommand, TCommandResult, TResponse>(
    IMapper<TRequest, TCommand> requestMapper,
    ISender sender,
    IMapper<TCommandResult, TResponse> responseMapper,
    IHttpContextAccessor httpContextAccessor
) : IHandler<TRequest, TCommand, TCommandResult, TResponse>
    where TCommand : IRequest<Result<TCommandResult>>
{
    public async Task<IResult> Handle(TRequest request)
    {
        var command = requestMapper.Map(request);

        var cancellationToken = httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;
        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            var response = responseMapper.Map(result.Value);
            return TypedResults.Ok(response);
        }

        var detail = result.Error.Description;
        return result.Error.Type switch
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

public class HandlerEmptyRequest<TCommand, TCommandResult, TResponse>(
    ISender sender,
    IMapper<TCommandResult, TResponse> responseMapper,
    IHttpContextAccessor httpContextAccessor
) : IHandlerEmptyRequest<TCommand, TCommandResult, TResponse>
    where TCommand : IRequest<Result<TCommandResult>>, new()
{
    public async Task<IResult> Handle()
    {
        var cancellationToken = httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;
        var result = await sender.Send(new TCommand(), cancellationToken);

        if (result.IsSuccess)
        {
            var response = responseMapper.Map(result.Value);
            return TypedResults.Ok(response);
        }

        var detail = result.Error.Description;
        return result.Error.Type switch
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

public class HandlerEmptyResponse<TRequest, TCommand>(
    IMapper<TRequest, TCommand> requestMapper,
    ISender sender,
    IHttpContextAccessor httpContextAccessor
) : IHandlerEmptyResponse<TRequest, TCommand>
    where TCommand : IRequest<Result>
{
    public async Task<IResult> Handle(TRequest request)
    {
        var command = requestMapper.Map(request);

        var cancellationToken = httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;
        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return TypedResults.Ok();
        }

        var detail = result.Error.Description;
        return result.Error.Type switch
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
