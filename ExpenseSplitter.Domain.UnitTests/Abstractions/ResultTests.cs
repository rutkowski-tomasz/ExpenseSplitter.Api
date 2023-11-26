﻿using ExpenseSplitter.Domain.Abstractions;

namespace ExpenseSplitter.Domain.UnitTests.Abstractions;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessInstance()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldCreateFailInstance()
    {
        var error = new Fixture().Create<Error>();
        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(error.Code);
        result.Error.Name.Should().Be(error.Name);
    }

    [Fact]
    public void Create_ShouldCreateSuccessInstance()
    {
        var result = Result.Create(123);

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
        result.Value.Should().Be(123);
    }

    [Fact]
    public void Create_ShouldCreateFailInstance_WhenValueIsNull()
    {
        var result = Result.Create((int?) null);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }
}
