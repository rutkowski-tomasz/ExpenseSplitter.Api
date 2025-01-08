using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Exceptions;
using ExpenseSplitter.Api.Application.Expenses.UpdateExpense;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Common;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;
using NSubstitute.ExceptionExtensions;

namespace ExpenseSplitter.Api.Application.UnitTests.Expenses;

public class UpdateExpenseCommandHandlerTests
{
    private readonly Fixture fixture;
    private readonly UpdateExpenseCommandHandler handler;
    private readonly IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    private readonly ISettlementUserRepository settlementUserRepository = Substitute.For<ISettlementUserRepository>();
    private readonly ISettlementRepository settlementRepository = Substitute.For<ISettlementRepository>();
    private readonly IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly Expense expense;
    private readonly Settlement settlement;

    public UpdateExpenseCommandHandlerTests()
    {
        fixture = CustomFixture.Create();

        settlement = fixture.Create<Settlement>();
        settlement.AddParticipant(fixture.Create<string>());
        settlement.AddParticipant(fixture.Create<string>());

        expense = Expense.Create(
            fixture.Create<string>(),
            fixture.Create<DateOnly>(),
            settlement.Id,
            settlement.Participants[0].Id,
            settlement.Participants.ToDictionary(
                x => x.Id,
                _ => fixture.Create<decimal>()
            )
        ).Value;

        settlementRepository
            .GetById(settlement.Id, Arg.Any<CancellationToken>())
            .Returns(settlement);

        expenseRepository
            .GetById(Arg.Any<ExpenseId>(), Arg.Any<CancellationToken>())
            .Returns(expense);

        settlementUserRepository
            .CanUserAccessSettlement(settlement.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        handler = new UpdateExpenseCommandHandler(
            expenseRepository,
            settlementUserRepository,
            settlementRepository,
            dateTimeProvider,
            unitOfWork
        );
    }

    [Fact]
    public async Task Handle_ShouldUpdateExpense_WhenValidRequest()
    {
        var request = fixture
            .Build<UpdateExpenseCommand>()
            .With(x => x.Allocations, fixture
                .Build<UpdateExpenseCommandAllocation>()
                .With(y => y.Id, (Guid?) null)
                .CreateMany()
            )
            .Create();

        var result = await handler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenExpenseNotFound()
    {
        var request = fixture.Create<UpdateExpenseCommand>();
        expenseRepository
            .GetById(Arg.Any<ExpenseId>(), Arg.Any<CancellationToken>())
            .Returns(null as Expense);

        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Should().Be(ExpenseErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserCannotAccessSettlement()
    {
        var request = fixture.Create<UpdateExpenseCommand>();

        settlementUserRepository
            .CanUserAccessSettlement(Arg.Any<SettlementId>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Should().Be(SettlementErrors.Forbidden);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUpdateTitleIsEmpty()
    {
        var request = fixture
            .Build<UpdateExpenseCommand>()
            .With(x => x.Title, "")
            .Create();

        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Type.Should().Be(ExpenseErrors.EmptyTitle.Type);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAllocationAmountIsNegative()
    {
        var request = fixture
            .Build<UpdateExpenseCommand>()
            .With(x => x.Allocations, new List<UpdateExpenseCommandAllocation>
            {
                new(Guid.CreateVersion7(), Guid.CreateVersion7(), -1M)
            })
            .Create();

        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Type.Should().Be(AmountErrors.NegativeValue.Type);
    }

    [Fact]
    public async Task Handle_ShouldUpdateExistingAllocations()
    {
        expense.AddAllocation(Amount.Create(2M).Value, settlement.Participants[0].Id);
        expense.AddAllocation(Amount.Create(2M).Value, settlement.Participants[1].Id);
            
        var request = fixture
            .Build<UpdateExpenseCommand>()
            .With(x => x.Allocations, new List<UpdateExpenseCommandAllocation>
            {
                new (expense.Allocations[1].Id.Value, settlement.Participants[1].Id.Value, 3M),
                new (expense.Allocations[0].Id.Value, settlement.Participants[0].Id.Value, 1M)
            })
            .Create();

        var result = await handler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
        expense.Allocations[0].Amount.Value.Should().Be(1M);
        expense.Allocations[1].Amount.Value.Should().Be(3M);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTryingToCreateAllocationWithNonPositiveAmount()
    {
        var request = fixture
            .Build<UpdateExpenseCommand>()
            .With(x => x.Allocations, new List<UpdateExpenseCommandAllocation>
            {
                new (null, Guid.CreateVersion7(), -0.01M),
                new (null, Guid.CreateVersion7(), 1M)
            })
            .Create();
        
        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Type.Should().Be(AmountErrors.NegativeValue.Type);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTryingToUpdateWithNegativeAmount()
    {
        expense.AddAllocation(Amount.Create(2M).Value, settlement.Participants[0].Id);
        expense.AddAllocation(Amount.Create(2M).Value, settlement.Participants[1].Id);

        var request = fixture
            .Build<UpdateExpenseCommand>()
            .With(x => x.Allocations, new List<UpdateExpenseCommandAllocation>
            {
                new (expense.Allocations[0].Id.Value, expense.Allocations[0].ParticipantId.Value, -1M),
                new (expense.Allocations[1].Id.Value, expense.Allocations[1].ParticipantId.Value, 3M)
            })
            .Create();

        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Type.Should().Be(AmountErrors.NegativeValue.Type);
    }
    
    [Fact]
    public async Task Handle_ShouldRemoveAllocation_WhenIsNotPresentInUpdateCommand()
    {
        expense.AddAllocation(Amount.Create(2M).Value, settlement.Participants[0].Id);
        expense.AddAllocation(Amount.Create(2M).Value, settlement.Participants[1].Id);

        var request = fixture
            .Build<UpdateExpenseCommand>()
            .With(x => x.Allocations, new List<UpdateExpenseCommandAllocation>
            {
                new (expense.Allocations[1].Id.Value, expense.Allocations[1].ParticipantId.Value, expense.Allocations[1].Amount.Value)
            })
            .Create();

        var result = await handler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
        expense.Allocations.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenDbConcurrencyExceptionOccurs()
    {
        expense.AddAllocation(Amount.Create(2M).Value, settlement.Participants[0].Id);
        expense.AddAllocation(Amount.Create(2M).Value, settlement.Participants[1].Id);
        
        var request = fixture
            .Build<UpdateExpenseCommand>()
            .With(x => x.Allocations, new List<UpdateExpenseCommandAllocation>
            {
                new (expense.Allocations[0].Id.Value, expense.Allocations[0].ParticipantId.Value, 1M),
                new (expense.Allocations[1].Id.Value, expense.Allocations[1].ParticipantId.Value, 3M)
            })
            .Create();

        unitOfWork
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new ConcurrencyException("concurrency", new Exception()));

        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Type.Should().Be(ConcurrencyException.ConcurrencyAppError.Type);
    }
}
