using System.Diagnostics.CodeAnalysis;

namespace ExpenseSplitter.Api.Domain.Abstractions;

public class Result
{
    protected Result(bool isSuccess, AppError appError)
    {
        IsSuccess = isSuccess;
        AppError = appError;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public AppError AppError { get; }

    public static Result Success() => new(true, AppError.None);

    public static Result Failure(AppError appError) => new(false, appError);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, AppError.None);

    public static Result<TValue> Failure<TValue>(AppError appError) => new(default, false, appError);

    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(AppError.None);

    public static implicit operator Result(AppError appError) => ToResult(appError);

    public static Result ToResult(AppError appError) => Failure(appError);
}

public class Result<TValue> : Result
{
    private readonly TValue? value;

    protected internal Result(TValue? value, bool isSuccess, AppError appError)
        : base(isSuccess, appError)
    {
        this.value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    public static implicit operator Result<TValue>(TValue? value) => Create(value);

    public static implicit operator Result<TValue>(AppError appError) => Failure<TValue>(appError);
}
