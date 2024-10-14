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

        idempotencyService
            .Setup(x => x.IsIdempotencyKeyInHeaders())
            .Returns(true);

        idempotencyKey = Guid.NewGuid();
        idempotencyService
            .Setup(x => x.TryParseIdempotencyKey(out idempotencyKey))
            .Returns(true);

        behavior = new IdempotentBehavior<TestCommand, Result<int>>(
            idempotencyService.Object
        );
    }

    private class TestCommand : IBaseCommand;

    [Fact]
    public async Task Handle_ShouldReturnNextValue_WhenIdempotencyKeyIsNotHeaders()
    {
        idempotencyService
            .Setup(x => x.IsIdempotencyKeyInHeaders())
            .Returns(false);

        var next = new RequestHandlerDelegate<Result<int>>(() => Task.FromResult(Result.Success(3)));

        var result = await behavior.Handle(new TestCommand(), next, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(3);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenIdempotencyKeyIsNotParseable()
    {
        Guid parsedIdempotencyKey;
        idempotencyService
            .Setup(x => x.TryParseIdempotencyKey(out parsedIdempotencyKey))
            .Returns(false);

        var next = new RequestHandlerDelegate<Result<int>>(() => Task.FromResult(Result.Success(43)));

        var result = await behavior.Handle(new TestCommand(), next, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.PreConditionFailed);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenIdempotencyKeyWasProcessed()
    {
        idempotencyService
            .Setup(x => x.IsIdempotencyKeyProcessed(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var next = new RequestHandlerDelegate<Result<int>>(() => Task.FromResult(Result.Success(43)));

        var result = await behavior.Handle(new TestCommand(), next, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }
    
    [Fact]
    public async Task Handle_ShouldSuccess()
    {
        var next = new RequestHandlerDelegate<Result<int>>(() => Task.FromResult(Result.Success(43)));

        var result = await behavior.Handle(new TestCommand(), next, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(43);
        
        idempotencyService.Verify(x => x.SaveIdempotencyKey(idempotencyKey, "TestCommand", It.IsAny<CancellationToken>()));
    }
}
