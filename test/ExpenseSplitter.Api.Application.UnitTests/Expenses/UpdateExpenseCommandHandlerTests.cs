using ExpenseSplitter.Api.Application.Abstraction.Clock;
using ExpenseSplitter.Api.Application.Expenses.UpdateExpense;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;
using ExpenseSplitter.Api.Domain.Shared;

namespace ExpenseSplitter.Api.Application.UnitTests.Expenses;

public class UpdateExpenseCommandHandlerTests
{
    private readonly Fixture fixture;
    private readonly UpdateExpenseCommandHandler updateExpenseCommandHandler;
    private readonly Mock<IExpenseRepository> expenseRepositoryMock;
    private readonly Mock<ISettlementUserRepository> settlementUserRepositoryMock;
    private readonly Mock<ISettlementRepository> settlementRepositoryMock;

    public UpdateExpenseCommandHandlerTests()
    {
        fixture = CustomFixutre.Create();
        expenseRepositoryMock = new Mock<IExpenseRepository>();
        var allocationRepositoryMock = new Mock<IAllocationRepository>();
        settlementUserRepositoryMock = new Mock<ISettlementUserRepository>();
        settlementRepositoryMock = new Mock<ISettlementRepository>();
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        updateExpenseCommandHandler = new UpdateExpenseCommandHandler(
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

        var result = await updateExpenseCommandHandler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenExpenseNotFound()
    {
        var request = fixture.Create<UpdateExpenseCommand>();
        expenseRepositoryMock.Setup(repo => repo.GetById(It.IsAny<ExpenseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Expense);

        var result = await updateExpenseCommandHandler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseErrors.NotFound);
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

        var result = await updateExpenseCommandHandler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SettlementErrors.Forbidden);
    }
}
