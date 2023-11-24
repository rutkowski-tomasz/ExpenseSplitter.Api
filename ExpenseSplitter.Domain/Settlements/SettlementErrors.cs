using ExpenseSplitter.Domain.Abstractions;

namespace ExpenseSplitter.Domain.Settlements;

public class SettlementErrors
{
    public static Error EmptyName = new(
        "Settlement.EmptyName",
        "Can't create settlement with empty name"
    );
}