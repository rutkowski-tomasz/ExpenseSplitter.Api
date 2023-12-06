namespace ExpenseSplitter.Api.Domain.SettlementUsers;

public record SettlementUserId(Guid Value)
{
    public static SettlementUserId New() => new(Guid.NewGuid());
}
