using ExpenseSplitter.Api.Application.Expenses.GetExpensesForSettlement;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Application.UnitTests.Expenses;

public class GetExpensesForSettlementQueryHandlerTests
{
    private readonly GetExpensesForSettlementQueryHandler handler;
    private readonly Fixture fixture = CustomFixture.Create();
    private readonly IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    private readonly GetExpensesForSettlementQuery query;

    public GetExpensesForSettlementQueryHandlerTests()
    {
        handler = new GetExpensesForSettlementQueryHandler(expenseRepository);
        query = fixture.Create<GetExpensesForSettlementQuery>();
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedExpenses()
    {
        var expenses = fixture.CreateMany<Expense>(2).ToList();
        MockExpenseRepositoryGetPagedBySettlementId(expenses);

        var result = await handler.Handle(query, default);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Expenses.Should().HaveCount(2);
        result.Value.Expenses.First().Title.Should().Be(expenses[0].Title);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenNoExpenses()
    {
        MockExpenseRepositoryGetPagedBySettlementId([]);

        var result = await handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Should().NotBeNull();
        result.Value.Expenses.Should().HaveCount(0);
    }

    private void MockExpenseRepositoryGetPagedBySettlementId(List<Expense> expenses)
    {
        expenseRepository.GetPagedBySettlementId(
                Arg.Is<SettlementId>(y => y.Value == query.SettlementId), 
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(expenses);
    }
}
