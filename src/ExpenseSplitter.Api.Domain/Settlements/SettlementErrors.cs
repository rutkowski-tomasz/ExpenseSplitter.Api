using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Settlements;

public static class SettlementErrors
{
    public static readonly Error EmptyName = new(
        ErrorType.Validation,
        "Can't create settlement with empty name"
    );

    public static readonly Error NotFound = new(
        ErrorType.NotFound,
        "The settlement with the specified identifier was not found"
    );

    public static readonly Error Forbidden = new(
        ErrorType.Forbidden,
        "Can't access this settlement"
    );

    public static readonly Error IfMatchHeaderConflict = new(
        ErrorType.PreConditionFailed,
        "ETag is not matching with If-Match header value"
    );

    public static readonly Error IfNoneMatchNotModified = new(
        ErrorType.NotModified,
        "ETag is matching with If-None-Match header value"
    );
}
