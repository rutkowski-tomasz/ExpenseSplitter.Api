using ExpenseSplitter.Api.Application.Expenses.GetExpense;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Expenses;

public class GetExpenseQueryHandlerTests
{
    private readonly GetExpenseQueryHandler handler;
    private readonly Fixture fixture = CustomFixture.Create();
    private readonly IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    private readonly ISettlementUserRepository settlementUserRepository = Substitute.For<ISettlementUserRepository>();
    private readonly GetExpenseQuery query;

    public GetExpenseQueryHandlerTests()
    {
        expenseRepository
            .GetById(Arg.Any<ExpenseId>(), Arg.Any<CancellationToken>())
            .Returns(fixture.Create<Expense>());

        settlementUserRepository
            .CanUserAccessSettlement(Arg.Any<SettlementId>(), Arg.Any<CancellationToken>())
            .Returns(true);

        query = fixture.Create<GetExpenseQuery>();

        handler = new GetExpenseQueryHandler(
            expenseRepository,
            settlementUserRepository
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExpenseDoesNotExist()
    {
        expenseRepository
            .GetById(Arg.Any<ExpenseId>(), Arg.Any<CancellationToken>())
            .Returns((Expense) default);

        var result = await handler.Handle(query, default);
        
        result.IsSuccess.Should().BeFalse();
        result.AppError.Type.Should().Be(ExpenseErrors.NotFound.Type);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserHasNoAccessToTheSettlement()
    {
        settlementUserRepository
            .CanUserAccessSettlement(Arg.Any<SettlementId>(), Arg.Any<CancellationToken>())
            .Returns(false);
        
        var result = await handler.Handle(query, default);
        
        result.IsSuccess.Should().BeFalse();
        result.AppError.Type.Should().Be(SettlementErrors.Forbidden.Type);
    }
}
