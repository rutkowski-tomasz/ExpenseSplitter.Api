using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Application.Abstractions.Idempotency;
using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Application.UnitTests.Abstractions.Idempotency;

public class IdempotentBehaviorTests
{
    private readonly IIdempotencyService idempotencyService;
    private readonly IdempotentBehavior<TestCommand, Result<int>> behavior;
    private readonly Guid idempotencyKey;

    public IdempotentBehaviorTests()
    {
        idempotencyService = Substitute.For<IIdempotencyService>();
        idempotencyKey = Guid.CreateVersion7();
        
        idempotencyService.GetIdempotencyKeyFromHeaders()
            .Returns(idempotencyKey);
        
        idempotencyService.GetProcessedRequest<string>(
            idempotencyKey, 
            Arg.Any<CancellationToken>()
        ).Returns((true, "{\"IsSuccess\":true,\"Value\":43,\"AppError\":null}"));

        behavior = new IdempotentBehavior<TestCommand, Result<int>>(idempotencyService);
    }

    private sealed class TestCommand : IBaseCommand;

    [Fact]
    public async Task Handle_ShouldReturnNextValue_WhenIdempotencyKeyIsNotHeaders()
    {
        idempotencyService.GetIdempotencyKeyFromHeaders()
            .Returns(Result.Failure<Guid>(new AppError(ErrorType.NotFound, "Idempotency key not found")));

        var next = new RequestHandlerDelegate<Result<int>>(cancellationToken => Task.FromResult(Result.Success(3)));

        var result = await behavior.Handle(new TestCommand(), next, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(3);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenIdempotencyKeyIsNotParseable()
    {
        idempotencyService.GetIdempotencyKeyFromHeaders()
            .Returns(Result.Failure<Guid>(new AppError(ErrorType.NotFound, "Idempotency key not found")));

        var next = new RequestHandlerDelegate<Result<int>>(cancellationToken => Task.FromResult(Result.Success(43)));

        var result = await behavior.Handle(new TestCommand(), next, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(43);
    }
    
    [Fact]
    public async Task Handle_ShouldSuccess()
    {
        idempotencyService.GetProcessedRequest<string>(
            idempotencyKey, 
            Arg.Any<CancellationToken>()
        ).Returns((false, null));

        var next = new RequestHandlerDelegate<Result<int>>(cancellationToken => Task.FromResult(Result.Success(43)));

        var result = await behavior.Handle(new TestCommand(), next, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(43);
        
        await idempotencyService.Received(1).SaveIdempotentRequest(
            idempotencyKey, 
            Arg.Any<string?>(), 
            Arg.Any<CancellationToken>()
        );
    }
}
