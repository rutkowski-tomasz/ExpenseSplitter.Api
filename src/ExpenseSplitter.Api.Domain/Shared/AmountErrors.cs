using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Shared;

public static class AmountErrors
{
    public static readonly Error NegativeValue = new(
        "AmountErrors.NonPositiveValue",
        "Money value must be positive"
    );
}
