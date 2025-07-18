using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Settlements;

public static class SettlementErrors
{
    public static readonly AppError EmptyName = new(
        ErrorType.Validation,
        "Can't create settlement with empty name"
    );

    public static readonly AppError NotFound = new(
        ErrorType.NotFound,
        "The settlement with the specified identifier was not found"
    );

    public static readonly AppError Forbidden = new(
        ErrorType.Forbidden,
        "Can't access this settlement"
    );

    public static readonly AppError IfMatchHeaderConflict = new(
        ErrorType.PreConditionFailed,
        "ETag is not matching with If-Match header value"
    );

    public static readonly AppError IfNoneMatchNotModified = new(
        ErrorType.NotModified,
        "ETag is matching with If-None-Match header value"
    );
    
    public static readonly AppError ParticipantNotFound = new(
        ErrorType.NotFound,
        "The participant with the specified identifier was not found in settlement"
    );
}
