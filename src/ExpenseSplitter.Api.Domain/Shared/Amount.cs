using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Shared;

public record Amount
{
    public static Amount operator +(Amount first, Amount second) => new(first.Value + second.Value);

    private Amount(decimal value) => Value = value;

    public decimal Value { get; init; }

    public static Result<Amount> Create(decimal value)
    {
        if (value < 0)
        {
            return Result.Failure<Amount>(AmountErrors.NegativeValue);
        }

        return new Amount(value);
    }

    public static Amount Zero() => new(0);

    public bool IsZero() => this == Zero();
}
