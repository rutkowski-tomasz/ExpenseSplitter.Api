using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Expenses.DeleteExpense;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Expenses;

public class DeleteExpenseCommandHandlerTests
{
    private readonly Fixture fixture;
    private readonly Mock<ISettlementUserRepository> settlementUserRepositoryMock;
    private readonly Mock<IExpenseRepository> expenseRepositoryMock;
    private readonly DeleteExpenseCommandHandler handler;

    public DeleteExpenseCommandHandlerTests()
    {
        fixture = CustomFixture.Create();
        settlementUserRepositoryMock = new Mock<ISettlementUserRepository>();
        expenseRepositoryMock = new Mock<IExpenseRepository>();
        Mock<ISettlementRepository> settlementRepositoryMock = new();
        Mock<IDateTimeProvider> dateTimeProviderMock = new();
        Mock<IUnitOfWork> unitOfWorkMock = new();

        expenseRepositoryMock
            .Setup(x => x.GetById(It.IsAny<ExpenseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.Create<Expense>());

        settlementUserRepositoryMock
            .Setup(x => x.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        settlementRepositoryMock
            .Setup(x => x.GetById(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.Create<Settlement>());

        handler = new DeleteExpenseCommandHandler(
            settlementUserRepositoryMock.Object,
            expenseRepositoryMock.Object,
            settlementRepositoryMock.Object,
            dateTimeProviderMock.Object,
            unitOfWorkMock.Object
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
        expenseRepositoryMock
            .Setup(x => x.GetById(It.IsAny<ExpenseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expense) null!);

        var command = fixture.Create<DeleteExpenseCommand>();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.AppError.Type.Should().Be(ExpenseErrors.NotFound.Type);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserCantAccessSettlement()
    {
        settlementUserRepositoryMock
            .Setup(x => x.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

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
