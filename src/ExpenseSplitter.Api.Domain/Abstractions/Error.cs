namespace ExpenseSplitter.Api.Domain.Abstractions;

public record Error(ErrorType Type, string Description)
{
    public static Error None = new(ErrorType.None, string.Empty);
}
