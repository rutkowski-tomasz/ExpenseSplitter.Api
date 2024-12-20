namespace ExpenseSplitter.Api.Domain.Settlements;

public record SettlementId(Guid Value)
{
    public static SettlementId New() => new(Guid.CreateVersion7());
}
