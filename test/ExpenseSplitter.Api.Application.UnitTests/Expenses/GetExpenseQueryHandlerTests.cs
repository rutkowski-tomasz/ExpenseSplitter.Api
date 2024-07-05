using ExpenseSplitter.Api.Application.Expenses.GetExpense;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Expenses;

public class GetExpenseQueryHandlerTests
{
    private readonly GetExpenseQueryHandler handler;
    private readonly Fixture fixture;
    private readonly Mock<IExpenseRepository> expenseRepositoryMock;
    private readonly Mock<IAllocationRepository> allocationRepository;
    private readonly Mock<ISettlementUserRepository> settlementUserRepository;

    public GetExpenseQueryHandlerTests()
    {
        fixture = CustomFixutre.Create();
        expenseRepositoryMock = new Mock<IExpenseRepository>();
        allocationRepository = new Mock<IAllocationRepository>();
        settlementUserRepository = new Mock<ISettlementUserRepository>();

        expenseRepositoryMock
            .Setup(x => x.GetById(It.IsAny<ExpenseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.Create<Expense>());

        settlementUserRepository
            .Setup(x => x.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        allocationRepository
            .Setup(x => x.GetAllByExpenseId(It.IsAny<ExpenseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.CreateMany<Allocation>());

        handler = new GetExpenseQueryHandler(
            expenseRepositoryMock.Object,
            allocationRepository.Object,
            settlementUserRepository.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnExpenseWithAllocations()
    {
        var query = fixture.Create<GetExpenseQuery>();
        var result = await handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Allocations.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExpenseDoesNotExist()
    {
        expenseRepositoryMock
            .Setup(x => x.GetById(It.IsAny<ExpenseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expense) null!);

        var query = fixture.Create<GetExpenseQuery>();
        var result = await handler.Handle(query, default);
        
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ExpenseErrors.NotFound.Type);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserHasNoAccessToTheSettlement()
    {
        settlementUserRepository
            .Setup(x => x.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var query = fixture.Create<GetExpenseQuery>();
        var result = await handler.Handle(query, default);
        
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(SettlementErrors.Forbidden.Type);
    }
}
