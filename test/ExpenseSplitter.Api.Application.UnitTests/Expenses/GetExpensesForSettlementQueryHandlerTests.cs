using ExpenseSplitter.Api.Application.Expenses.GetExpensesForSettlement;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.Shared;

namespace ExpenseSplitter.Api.Application.UnitTests.Expenses;

public class GetExpensesForSettlementQueryHandlerTests
{
    private readonly GetExpensesForSettlementQueryHandler getExpensesForSettlementQueryHandler;
    private readonly Mock<IExpenseRepository> expenseRepositoryMock;

    public GetExpensesForSettlementQueryHandlerTests()
    {
        expenseRepositoryMock = new Mock<IExpenseRepository>();
        
        getExpensesForSettlementQueryHandler = new GetExpensesForSettlementQueryHandler(
            expenseRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedExpenses()
    {
        var request = new Fixture().Create<GetExpensesForSettlementQuery>();
        var expenses = new Fixture()
            .Build<Expense>()
            .FromFactory(
                (string name, Amount amount, DateTime date)
                    => Expense.Create(name, amount, date, SettlementId.New(), ParticipantId.New()).Value
            )
            .CreateMany(2)
            .ToList();
        expenseRepositoryMock
            .Setup(x => x.GetAllWithSettlementId(
                It.Is<SettlementId>(y => y.Value == request.SettlementId), 
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(expenses);

        var result = await getExpensesForSettlementQueryHandler.Handle(request, default);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Expenses.Should().HaveCount(2);
        result.Value.Expenses.First().Title.Should().Be(expenses.First().Title);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenNoExpenses()
    {
        var request = new Fixture().Create<GetExpensesForSettlementQuery>();
        var expenses = Enumerable.Empty<Expense>().ToList();
        expenseRepositoryMock
            .Setup(x => x.GetAllWithSettlementId(
                It.Is<SettlementId>(y => y.Value == request.SettlementId), 
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(expenses);

        var result = await getExpensesForSettlementQueryHandler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
        result.Should().NotBeNull();
        result.Value.Expenses.Should().HaveCount(0);
    }
}