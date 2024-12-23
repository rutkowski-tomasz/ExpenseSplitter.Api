using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Common;

public record Amount
{
    public static Amount operator +(Amount first, Amount second)
    {
        ArgumentNullException.ThrowIfNull(first);
        return first.Add(second);
    }

    private Amount Add(Amount other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return new Amount(Value + other.Value);
    }

    private Amount(decimal value) => Value = value;

    public decimal Value { get; init; }

    public static Result<Amount> Create(decimal value)
    {
        return value < 0
            ? Result.Failure<Amount>(AmountErrors.NegativeValue)
            : new Amount(value);
    }

    public static Amount Zero() => new(0);

    public bool IsZero() => this == Zero();
}
