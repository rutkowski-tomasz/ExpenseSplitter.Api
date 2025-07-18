namespace ExpenseSplitter.Api.Domain.Abstractions;

public record AppError(ErrorType Type, string Description)
{
    public static readonly AppError None = new(ErrorType.None, string.Empty);
}
