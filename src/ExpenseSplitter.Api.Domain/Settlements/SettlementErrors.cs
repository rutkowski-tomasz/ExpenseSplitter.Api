using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Settlements;

public static class SettlementErrors
{
    public static readonly Error EmptyName = new(
        "Settlement.EmptyName",
        "Can't create settlement with empty name"
    );

    public static readonly Error NotFound = new(
        "Settlement.NotFound",
        "The settlement with the specified identifier was not found"
    );

    public static readonly Error Forbidden = new(
        "Settlement.Forbidden",
        "Can't access this settlement"
    );
}