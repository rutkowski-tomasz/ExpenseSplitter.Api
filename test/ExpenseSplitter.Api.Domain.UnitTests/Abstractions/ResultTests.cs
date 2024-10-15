using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.UnitTests.Abstractions;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessInstance()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.AppError.Should().Be(AppError.None);
    }

    [Fact]
    public void Failure_ShouldCreateFailInstance()
    {
        var error = new Fixture().Create<AppError>();
        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.AppError.Type.Should().Be(error.Type);
        result.AppError.Description.Should().Be(error.Description);
    }

    [Fact]
    public void Create_ShouldCreateSuccessInstance()
    {
        var result = Result.Create(123);

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.AppError.Should().Be(AppError.None);
        result.Value.Should().Be(123);
    }

    [Fact]
    public void Create_ShouldCreateFailInstance_WhenValueIsNull()
    {
        var result = Result.Create((int?) null);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.AppError.Should().Be(AppError.None);
    }
}
