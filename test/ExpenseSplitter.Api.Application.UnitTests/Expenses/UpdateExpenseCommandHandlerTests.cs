using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Exceptions;
using ExpenseSplitter.Api.Application.Expenses.UpdateExpense;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Common;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Expenses;

public class UpdateExpenseCommandHandlerTests
{
    private readonly Fixture fixture;
    private readonly UpdateExpenseCommandHandler handler;
    private readonly Mock<IExpenseRepository> expenseRepositoryMock;
    private readonly Mock<ISettlementUserRepository> settlementUserRepositoryMock;
    private readonly Mock<IAllocationRepository> allocationRepositoryMock;
    private readonly Mock<IUnitOfWork> unitOfWorkMock;

    public UpdateExpenseCommandHandlerTests()
    {
        fixture = CustomFixture.Create();
        expenseRepositoryMock = new Mock<IExpenseRepository>();
        allocationRepositoryMock = new Mock<IAllocationRepository>();
        settlementUserRepositoryMock = new Mock<ISettlementUserRepository>();
        Mock<ISettlementRepository> settlementRepositoryMock = new();
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        unitOfWorkMock = new Mock<IUnitOfWork>();

        var expense = fixture.Create<Expense>();
        var settlement = fixture.Create<Settlement>();

        settlementRepositoryMock
            .Setup(x => x.GetById(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(settlement);

        expenseRepositoryMock
            .Setup(repo => repo.GetById(It.IsAny<ExpenseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expense);

        settlementUserRepositoryMock.Setup(repo => repo.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        handler = new UpdateExpenseCommandHandler(
            expenseRepositoryMock.Object,
            settlementUserRepositoryMock.Object,
            allocationRepositoryMock.Object,
            settlementRepositoryMock.Object,
            dateTimeProviderMock.Object,
            unitOfWorkMock.Object
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
        expenseRepositoryMock.Setup(repo => repo.GetById(It.IsAny<ExpenseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Expense);

        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Should().Be(ExpenseErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserCannotAccessSettlement()
    {
        var request = fixture.Create<UpdateExpenseCommand>();
        var expense = fixture.Create<Expense>();

        expenseRepositoryMock.Setup(repo => repo.GetById(It.IsAny<ExpenseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expense);
        settlementUserRepositoryMock.Setup(repo => repo.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

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
                new(Guid.NewGuid(), Guid.NewGuid(), -1M)
            })
            .Create();

        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Type.Should().Be(AmountErrors.NegativeValue.Type);
    }

    [Fact]
    public async Task Handle_ShouldUpdateExistingAllocations()
    {
        var allocation1 = fixture.Create<Allocation>();
        var allocation2 = fixture.Create<Allocation>();

        var request = fixture
            .Build<UpdateExpenseCommand>()
            .With(x => x.Allocations, new List<UpdateExpenseCommandAllocation>
            {
                new (allocation1.Id.Value, allocation1.ParticipantId.Value, 1M),
                new (allocation2.Id.Value, allocation2.ParticipantId.Value, 3M)
            })
            .Create();

        allocationRepositoryMock
            .Setup(x => x.GetAllByExpenseId(It.IsAny<ExpenseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Allocation> { allocation1, allocation2 });

        var result = await handler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
        allocation1.Amount.Value.Should().Be(1M);
        allocation2.Amount.Value.Should().Be(3M);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTryingToUpdateWithEmptyTitle()
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
    public async Task Handle_ShouldReturnFailure_WhenTryingToCreateAllocationWithNonPositiveAmount()
    {
        var request = fixture
            .Build<UpdateExpenseCommand>()
            .With(x => x.Allocations, new List<UpdateExpenseCommandAllocation>
            {
                new (null, Guid.NewGuid(), -0.01M),
                new (null, Guid.NewGuid(), 1M)
            })
            .Create();
        
        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Type.Should().Be(AmountErrors.NegativeValue.Type);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTryingToUpdateWithNegativeAmount()
    {
        var allocation1 = fixture.Create<Allocation>();
        var allocation2 = fixture.Create<Allocation>();

        var request = fixture
            .Build<UpdateExpenseCommand>()
            .With(x => x.Allocations, new List<UpdateExpenseCommandAllocation>
            {
                new (allocation1.Id.Value, allocation1.ParticipantId.Value, -1M),
                new (allocation2.Id.Value, allocation2.ParticipantId.Value, 3M)
            })
            .Create();

        allocationRepositoryMock
            .Setup(x => x.GetAllByExpenseId(It.IsAny<ExpenseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Allocation> { allocation1, allocation2 });

        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Type.Should().Be(AmountErrors.NegativeValue.Type);
    }
    
    [Fact]
    public async Task Handle_ShouldRemoveAllocation_WhenIsNotPresentInUpdateCommand()
    {
        var allocation1 = fixture.Create<Allocation>();
        var allocation2 = fixture.Create<Allocation>();

        var request = fixture
            .Build<UpdateExpenseCommand>()
            .With(x => x.Allocations, new List<UpdateExpenseCommandAllocation>
            {
                new (allocation2.Id.Value, allocation2.ParticipantId.Value, allocation2.Amount.Value)
            })
            .Create();

        allocationRepositoryMock
            .Setup(x => x.GetAllByExpenseId(It.IsAny<ExpenseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Allocation> { allocation1, allocation2 });

        var result = await handler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
        allocationRepositoryMock.Verify(x => x.Remove(It.Is<Allocation>(y => y.Id == allocation1.Id)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenDbConcurrencyExceptionOccurs()
    {
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConcurrencyException("concurrency", new Exception()));
        
        var request = fixture.Create<UpdateExpenseCommand>();
        var result = await handler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.AppError.Type.Should().Be(ConcurrencyException.ConcurrencyAppError.Type);
    }
}
