﻿using ExpenseSplitter.Application.Abstractions.Behaviors;
using ExpenseSplitter.Application.Abstractions.Cqrs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseSplitter.Application.UnitTests.Abstractions.Behaviors;

public class LoggingBehaviorTests
{
    private readonly Mock<ILogger<TestCommand>> loggerMock;
    private readonly LoggingBehavior<TestCommand, int> loggingBehavior;
    private readonly TestCommand request;

    public record TestCommand : IBaseCommand;

    public LoggingBehaviorTests()
    {
        loggerMock = new Mock<ILogger<TestCommand>>();
        loggingBehavior = new LoggingBehavior<TestCommand, int>(
            loggerMock.Object
        );

        request = new Fixture().Create<TestCommand>();
    }

    [Fact]
    public async Task Handle_ShouldLogProcessingInformation()
    {
        var next = new RequestHandlerDelegate<int>(() => Task.FromResult(1));

        var response = await loggingBehavior.Handle(request, next, default);

        response.Should().Be(1);

        loggerMock.Verify(logger => logger.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Executing command {nameof(TestCommand)}")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);

        loggerMock.Verify(logger => logger.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Command {nameof(TestCommand)} processed successfully")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);

        loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenDelegateThrowsAnException()
    {
        var next = new RequestHandlerDelegate<int>(() => throw new Exception());

        var act = async () => await loggingBehavior.Handle(request, next, default);

        await act.Should().ThrowAsync<Exception>();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        loggerMock.Verify(logger => logger.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Executing command {nameof(TestCommand)}")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);

        loggerMock.Verify(logger => logger.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Command {nameof(TestCommand)} processing failed")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        loggerMock.VerifyNoOtherCalls();
    }
}
