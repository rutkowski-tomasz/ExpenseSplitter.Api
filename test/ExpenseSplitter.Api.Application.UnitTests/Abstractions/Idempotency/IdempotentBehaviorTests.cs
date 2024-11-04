using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Application.Abstractions.Idempotency;
using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Application.UnitTests.Abstractions.Idempotency;

public class IdempotentBehaviorTests
{
    private readonly Mock<IIdempotencyService> idempotencyService;
    private readonly IdempotentBehavior<TestCommand, Result<int>> behavior;
    private Guid idempotencyKey;

    public IdempotentBehaviorTests()
    {
        idempotencyService = new Mock<IIdempotencyService>();

        idempotencyKey = Guid.NewGuid();
        idempotencyService
            .Setup(x => x.GetIdempotencyKeyFromHeaders())
            .Returns(idempotencyKey);
        
        idempotencyService
            .Setup(x => x.GetProcessedRequest<string>(idempotencyKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, "{\"IsSuccess\":true,\"Value\":43,\"AppError\":null}"));

        behavior = new IdempotentBehavior<TestCommand, Result<int>>(
            idempotencyService.Object
        );
    }

    private class TestCommand : IBaseCommand;

    [Fact]
    public async Task Handle_ShouldReturnNextValue_WhenIdempotencyKeyIsNotHeaders()
    {
        idempotencyService
            .Setup(x => x.GetIdempotencyKeyFromHeaders())
            .Returns(Result.Failure<Guid>(new AppError(ErrorType.NotFound, "Idempotency key not found")));

        var next = new RequestHandlerDelegate<Result<int>>(() => Task.FromResult(Result.Success(3)));

        var result = await behavior.Handle(new TestCommand(), next, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(3);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenIdempotencyKeyIsNotParseable()
    {
        idempotencyService
            .Setup(x => x.GetIdempotencyKeyFromHeaders())
            .Returns(Result.Failure<Guid>(new AppError(ErrorType.NotFound, "Idempotency key not found")));

        var next = new RequestHandlerDelegate<Result<int>>(() => Task.FromResult(Result.Success(43)));

        var result = await behavior.Handle(new TestCommand(), next, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(43);
    }
    
    [Fact]
    public async Task Handle_ShouldSuccess()
    {
        idempotencyService
            .Setup(x => x.GetProcessedRequest<string>(idempotencyKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, null));

        var next = new RequestHandlerDelegate<Result<int>>(() => Task.FromResult(Result.Success(43)));

        var result = await behavior.Handle(new TestCommand(), next, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(43);
        
        idempotencyService.Verify(x =>
            x.SaveIdempotentRequest(idempotencyKey, It.IsAny<string?>(), It.IsAny<CancellationToken>())
        );
    }
}
