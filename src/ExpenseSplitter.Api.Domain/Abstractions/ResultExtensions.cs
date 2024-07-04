namespace ExpenseSplitter.Api.Domain.Abstractions;

public static class ResultExtensions
{
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> func) =>
        result.IsSuccess ? func(result.Value) : Result.Failure<TOut>(result.Error);

    public static Result<TOut> TryCatch<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> func, Error error)
    {
        try
        {
            return result.IsSuccess
                ? Result.Success(func(result.Value))
                : Result.Failure<TOut>(result.Error);
        }
        catch
        {
            return Result.Failure<TOut>(error);
        }
    }

    public static Result<TIn> Tap<TIn>(this Result<TIn> result, Action<TIn> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }

        return result;
    }

    public static TOut Match<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> onSuccess,
        Func<Error, TOut> onFailure
    ) => result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
}
