using ExpenseSplitter.Domain.Abstractions;

namespace ExpenseSplitter.Domain.Settlements;

public class Settlement : Entity<SettlementId>
{
    private Settlement(
        SettlementId id,
        string name
    ) : base(id)
    {
    }

	public string Name { get; private set; }

    public static Settlement Create(string name)
    {
        var settlement = new Settlement(SettlementId.New(), name);

        return settlement;
    }
}
