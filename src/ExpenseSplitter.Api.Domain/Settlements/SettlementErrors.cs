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
}
