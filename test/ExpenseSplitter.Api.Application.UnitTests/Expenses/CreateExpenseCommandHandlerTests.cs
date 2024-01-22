using ExpenseSplitter.Api.Application.Abstraction.Clock;
using ExpenseSplitter.Api.Application.Expenses.CreateExpense;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Expenses;

public class CreateExpenseCommandHandlerTests
{
    private readonly CreateExpenseCommandHandler createExpenseCommandHandler;
    private readonly Fixture fixture;
    private readonly Mock<ISettlementUserRepository> settlementUserRepositoryMock;
    private readonly Mock<IParticipantRepository> participantRepositoryMock;
    private readonly Mock<ISettlementRepository> settlementRepositoryMock;

    public CreateExpenseCommandHandlerTests()
    {
        fixture = CustomFixutre.Create();
        settlementUserRepositoryMock = new Mock<ISettlementUserRepository>();
        var expenseRepositoryMock = new Mock<IExpenseRepository>();
        var allocationRepositoryMock = new Mock<IAllocationRepository>();
        participantRepositoryMock = new Mock<IParticipantRepository>();
        settlementRepositoryMock = new Mock<ISettlementRepository>();
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        
        createExpenseCommandHandler = new CreateExpenseCommandHandler(
            settlementUserRepositoryMock.Object,
            expenseRepositoryMock.Object,
            allocationRepositoryMock.Object,
            participantRepositoryMock.Object,
            settlementRepositoryMock.Object,
            dateTimeProviderMock.Object,
            unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldProcess()
    {
        participantRepositoryMock.Setup(x => x.AreAllParticipantsInSettlement(
            It.IsAny<SettlementId>(),
            It.IsAny<IEnumerable<ParticipantId>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        settlementUserRepositoryMock.Setup(x => x.CanUserAccessSettlement(
            It.IsAny<SettlementId>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        settlementRepositoryMock.Setup(x => x.GetById(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.Create<Settlement>());

        var request = fixture.Create<CreateExpenseCommand>();

        var result = await createExpenseCommandHandler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnForbiddenFailure_WhenUserIsNotParticipatingInSettlement()
    {
        participantRepositoryMock.Setup(x => x.AreAllParticipantsInSettlement(
            It.IsAny<SettlementId>(),
            It.IsAny<IEnumerable<ParticipantId>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        settlementUserRepositoryMock.Setup(x => x.CanUserAccessSettlement(
            It.IsAny<SettlementId>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(false);

        var request = fixture.Create<CreateExpenseCommand>();
            
        var result = await createExpenseCommandHandler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SettlementErrors.Forbidden);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyNameFailure_WhenExpenseTitleIsEmpty()
    {
        participantRepositoryMock.Setup(x => x.AreAllParticipantsInSettlement(
            It.IsAny<SettlementId>(),
            It.IsAny<IEnumerable<ParticipantId>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        settlementUserRepositoryMock.Setup(x => x.CanUserAccessSettlement(
            It.IsAny<SettlementId>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        settlementRepositoryMock.Setup(x => x.GetById(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.Create<Settlement>());

        var request = fixture
            .Build<CreateExpenseCommand>()
            .With(x => x.Title, string.Empty)
            .Create();

        var result = await createExpenseCommandHandler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseErrors.EmptyTitle);
    }
    
    
    [Fact]
    public async Task Handle_ShouldReturnNotFoundFailure_WhenNotAllParticipantsAreInTheSettlement()
    {
        participantRepositoryMock.Setup(x => x.AreAllParticipantsInSettlement(
            It.IsAny<SettlementId>(),
            It.IsAny<IEnumerable<ParticipantId>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(false);

        settlementUserRepositoryMock.Setup(x => x.CanUserAccessSettlement(
            It.IsAny<SettlementId>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        var request = fixture.Create<CreateExpenseCommand>();
            
        var result = await createExpenseCommandHandler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ParticipantErrors.NotFound);
    }
}
