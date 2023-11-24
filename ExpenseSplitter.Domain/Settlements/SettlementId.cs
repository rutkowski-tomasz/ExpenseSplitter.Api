namespace ExpenseSplitter.Domain.Settlements;

public record SettlementId(Guid Value)
{
    public static SettlementId New() => new(Guid.NewGuid());
}
