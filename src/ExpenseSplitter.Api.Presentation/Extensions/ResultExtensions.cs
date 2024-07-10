using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Presentation.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResult(this Result result)
    {
        return result.IsSuccess ? Results.Ok() : MapResultError(result.Error);
    }

    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        return result.IsSuccess ? Results.Ok(result.Value) : MapResultError(result.Error);
    }

    private static IResult MapResultError(Error error)
    {
        return error.Type switch
        {
            ErrorType.Validation => Results.BadRequest(error.Description),
            ErrorType.NotFound => Results.NotFound(error.Description),
            ErrorType.Forbidden => Results.Forbid(),
            ErrorType.BadRequest => Results.BadRequest(error.Description),
            ErrorType.PreConditionFailed => Results.Problem(error.Description, statusCode: StatusCodes.Status412PreconditionFailed),
            ErrorType.Conflict => Results.Conflict(error.Description),
            ErrorType.BadGateway => Results.Problem(error.Description, statusCode: StatusCodes.Status502BadGateway),
            ErrorType.Unauthorized => Results.Unauthorized(),
            ErrorType.NotModified => Results.StatusCode(StatusCodes.Status304NotModified),
            _ => Results.Problem(error.Description, statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
