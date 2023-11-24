namespace ExpenseSplitter.Domain.Settlements;

public class Settlement
{
    private Settlement()
    {

    }

	public SettlementId Id { get; private set; }

	public string Name { get; private set; } = string.Empty;

    public static Settlement Create(string name)
    {
        var settlement = new Settlement()
        {
            Id = new SettlementId(Guid.NewGuid()),
            Name = name,
        };

        return settlement;
    }
}
