using ExpenseSplitter.Domain.Abstractions;

namespace ExpenseSplitter.Domain.Settlements;

public class SettlementErrors
{
    public static Error EmptyName = new(
        "Settlement.EmptyName",
        "Can't create settlement with empty name"
    );

    public static Error NotFound = new(
        "Settlement.NotFound",
        "The settlement with the specified identifier was not found"
    );
}