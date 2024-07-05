namespace ExpenseSplitter.Api.Domain.Abstractions;

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Forbidden,
    BadRequest,
    ServerInternalError,
    PreConditionFailed,
    Conflict,
    BadGateway,
    Unauthorized,
}
