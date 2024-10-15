using ExpenseSplitter.Api.Application.Abstractions.Behaviors;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseSplitter.Api.Application.UnitTests.Abstractions.Behaviors;

public class LoggingBehaviorTests
{
    private readonly Mock<ILogger<TestCommand>> loggerMock;
    private readonly LoggingBehavior<TestCommand, Result<int>> loggingBehavior;
    private readonly TestCommand request;

    // ReSharper disable once MemberCanBePrivate.Global
    public record TestCommand : IRequest;

    public LoggingBehaviorTests()
    {
        loggerMock = new Mock<ILogger<TestCommand>>();
        loggingBehavior = new LoggingBehavior<TestCommand, Result<int>>(
            loggerMock.Object
        );

        request = new TestCommand();
    }

    [Fact]
    public async Task Handle_ShouldLogProcessingInformation()
    {
        var next = new RequestHandlerDelegate<Result<int>>(() => Task.FromResult(Result.Success(1)));

        var result = await loggingBehavior.Handle(request, next, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(1);

        loggerMock.Verify(logger => logger.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Processing request {nameof(TestCommand)}")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);

        loggerMock.Verify(logger => logger.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Request success {nameof(TestCommand)}")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);

        loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenDelegateThrowsAnException()
    {
        var next = new RequestHandlerDelegate<Result<int>>(() => Task.FromResult(Result.Failure<int>(SettlementErrors.Forbidden)));

        var result = await loggingBehavior.Handle(request, next, default);

        result.IsFailure.Should().BeTrue();

        loggerMock.Verify(logger => logger.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Processing request {nameof(TestCommand)}")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);

        loggerMock.Verify(logger => logger.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Request failure {nameof(TestCommand)}")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);

        loggerMock.VerifyNoOtherCalls();
    }
}
