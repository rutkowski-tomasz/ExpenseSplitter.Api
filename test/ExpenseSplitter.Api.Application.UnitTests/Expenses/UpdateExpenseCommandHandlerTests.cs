using System.Net.Http.Headers;
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
    private readonly UpdateExpenseCommandHandler updateExpenseCommandHandler;
    private readonly Mock<IExpenseRepository> expenseRepositoryMock;
    private readonly Mock<IAllocationRepository> allocationRepositoryMock;
    private readonly Mock<ISettlementUserRepository> settlementUserRepositoryMock;
    private readonly Mock<IUnitOfWork> unitOfWorkMock;

    public UpdateExpenseCommandHandlerTests()
    {
        expenseRepositoryMock = new Mock<IExpenseRepository>();
        allocationRepositoryMock = new Mock<IAllocationRepository>();
        settlementUserRepositoryMock = new Mock<ISettlementUserRepository>();
        unitOfWorkMock = new Mock<IUnitOfWork>();

        updateExpenseCommandHandler = new UpdateExpenseCommandHandler(
            expenseRepositoryMock.Object,
            settlementUserRepositoryMock.Object,
            allocationRepositoryMock.Object,
            unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldUpdateExpense_WhenValidRequest()
    {
        var request = new Fixture().Create<UpdateExpenseCommand>();
        var expense = new Fixture()
            .Build<Expense>()
            .FromFactory(
                (string title, Amount amount, DateTime date)
                    => Expense.Create(title, amount, date, SettlementId.New(), ParticipantId.New()).Value
            )
            .Create();

        expenseRepositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<ExpenseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expense);
        settlementUserRepositoryMock.Setup(repo => repo.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await updateExpenseCommandHandler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenExpenseNotFound()
    {
        var request = new Fixture().Create<UpdateExpenseCommand>();
        expenseRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<ExpenseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expense)null);

        var result = await updateExpenseCommandHandler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserCannotAccessSettlement()
    {
        var request = new Fixture().Create<UpdateExpenseCommand>();
        var expense = new Fixture()
            .Build<Expense>()
            .FromFactory(
                (string title, Amount amount, DateTime date)
                    => Expense.Create(title, amount, date, SettlementId.New(), ParticipantId.New()).Value
            )
            .Create();

        expenseRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<ExpenseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expense);
        settlementUserRepositoryMock.Setup(repo => repo.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await updateExpenseCommandHandler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SettlementErrors.Forbidden);
    }
}
