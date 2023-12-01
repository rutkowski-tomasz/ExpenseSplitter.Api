namespace ExpenseSplitter.Api.Domain.Shared;

public record Amount(decimal Value)
{
    public static Amount Zero() => new(0);

    public bool IsZero() => this == Zero();
}
