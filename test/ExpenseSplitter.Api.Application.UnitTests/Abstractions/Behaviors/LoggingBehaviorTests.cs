using ExpenseSplitter.Api.Application.Abstractions.Behaviors;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseSplitter.Api.Application.UnitTests.Abstractions.Behaviors;

public class LoggingBehaviorTests
{
    private readonly ILogger<TestCommand> logger;
    private readonly LoggingBehavior<TestCommand, Result<int>> loggingBehavior;
    private readonly TestCommand request;

    public record TestCommand : IRequest;

    public LoggingBehaviorTests()
    {
        logger = Substitute.For<ILogger<TestCommand>>();
        loggingBehavior = new LoggingBehavior<TestCommand, Result<int>>(logger);
        request = new TestCommand();
    }

    [Fact]
    public async Task Handle_ShouldLogProcessingInformation()
    {
        var next = new RequestHandlerDelegate<Result<int>>(() => Task.FromResult(Result.Success(1)));

        var result = await loggingBehavior.Handle(request, next, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(1);

        logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<Arg.AnyType>((object v) => v.ToString()!.Contains($"Processing request {nameof(TestCommand)}")),
            Arg.Any<Exception>(),
            Arg.Any<Func<Arg.AnyType, Exception?, string>>());

        logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<Arg.AnyType>((object v) => v.ToString()!.Contains($"Request success {nameof(TestCommand)}")),
            Arg.Any<Exception>(),
            Arg.Any<Func<Arg.AnyType, Exception?, string>>());

        logger.ReceivedCalls().Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenDelegateThrowsAnException()
    {
        var next = new RequestHandlerDelegate<Result<int>>(() => 
            Task.FromResult(Result.Failure<int>(SettlementErrors.Forbidden)));

        var result = await loggingBehavior.Handle(request, next, default);

        result.IsFailure.Should().BeTrue();

        logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<Arg.AnyType>((object v) => v.ToString()!.Contains($"Processing request {nameof(TestCommand)}")),
            Arg.Any<Exception>(),
            Arg.Any<Func<Arg.AnyType, Exception?, string>>());

        logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<Arg.AnyType>((object v) => v.ToString()!.Contains($"Request failure {nameof(TestCommand)}")),
            Arg.Any<Exception>(),
            Arg.Any<Func<Arg.AnyType, Exception?, string>>());

        logger.ReceivedCalls().Should().HaveCount(2);
    }
}
