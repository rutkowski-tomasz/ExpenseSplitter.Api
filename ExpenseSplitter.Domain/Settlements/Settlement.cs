using ExpenseSplitter.Domain.Abstractions;

namespace ExpenseSplitter.Domain.Settlements;

public sealed class Settlement : Entity<SettlementId>
{
    private Settlement(
        SettlementId id,
        string name
    ) : base(id)
    {
        Name = name;
    }

	public string Name { get; private set; }

    public static Result<Settlement> Create(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return Result.Failure<Settlement>(SettlementErrors.EmptyName);
        }
        
        var settlement = new Settlement(SettlementId.New(), name);

        return settlement;
    }
}