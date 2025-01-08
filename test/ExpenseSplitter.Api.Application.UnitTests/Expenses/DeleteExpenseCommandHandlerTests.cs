using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Expenses.DeleteExpense;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Expenses;

public class DeleteExpenseCommandHandlerTests
{
    private readonly DeleteExpenseCommandHandler handler;
    private readonly ISettlementUserRepository settlementUserRepository = Substitute.For<ISettlementUserRepository>();
    private readonly IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    private readonly ISettlementRepository settlementRepository = Substitute.For<ISettlementRepository>();
    private readonly IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly Fixture fixture = CustomFixture.Create();

    public DeleteExpenseCommandHandlerTests()
    {
        var expense = fixture.Create<Expense>();
        expenseRepository
            .GetById(Arg.Any<ExpenseId>(), Arg.Any<CancellationToken>())
            .Returns(expense);

        settlementUserRepository
            .CanUserAccessSettlement(Arg.Any<SettlementId>(), Arg.Any<CancellationToken>())
            .Returns(true);

        settlementRepository
            .GetById(Arg.Any<SettlementId>(), Arg.Any<CancellationToken>())
            .Returns(fixture.Create<Settlement>());

        handler = new DeleteExpenseCommandHandler(
            settlementUserRepository,
            expenseRepository,
            settlementRepository,
            dateTimeProvider,
            unitOfWork
        );
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenIdIsEmpty()
    {
        var command = fixture
            .Build<DeleteExpenseCommand>()
            .With(x => x.Id, Guid.Empty)
            .Create();

        var validator = new DeleteExpenseCommandValidator();
        var result = await validator.ValidateAsync(command);

        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Id));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenExpenseWitIdDoesNotExist()
    {
        expenseRepository
            .GetById(Arg.Any<ExpenseId>(), Arg.Any<CancellationToken>())
            .Returns((Expense)default);

        var command = fixture.Create<DeleteExpenseCommand>();
        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.AppError.Type.Should().Be(ExpenseErrors.NotFound.Type);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserCantAccessSettlement()
    {
        settlementUserRepository
            .CanUserAccessSettlement(Arg.Any<SettlementId>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var command = fixture.Create<DeleteExpenseCommand>();
        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.AppError.Type.Should().Be(SettlementErrors.Forbidden.Type);
    }

    [Fact]
    public async Task Handle_ShoulSuccess()
    {
        var command = fixture.Create<DeleteExpenseCommand>();

        var response = await handler.Handle(command, default);

        response.IsSuccess.Should().BeTrue();
    }
}
