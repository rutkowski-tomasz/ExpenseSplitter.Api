using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Common;

public static class AmountErrors
{
    public static readonly AppError NegativeValue = new(
        ErrorType.Validation,
        "Money value must be positive"
    );
}
