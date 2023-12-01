using ExpenseSplitter.Api.Application.Expenses.CreateExpense;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.ExpenseAllocations;
using ExpenseSplitter.Api.Domain.ExpenseAllocations.Services;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Application.UnitTests.Expenses;

public class CreateExpenseCommandHandlerTests
{
    private readonly CreateExpenseCommandHandler createExpenseCommandHandler;
    private readonly Mock<IParticipantRepository> participantRepositoryMock;

    public CreateExpenseCommandHandlerTests()
    {
        var expenseRepositoryMock = new Mock<IExpenseRepository>();
        var expenseAllocationRepositoryMock = new Mock<IExpenseAllocationRepository>();
        var expenseAllocationServiceMock = new Mock<IExpenseAllocationService>();
        participantRepositoryMock = new Mock<IParticipantRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        
        createExpenseCommandHandler = new CreateExpenseCommandHandler(
            expenseRepositoryMock.Object,
            expenseAllocationRepositoryMock.Object,
            expenseAllocationServiceMock.Object,
            participantRepositoryMock.Object,
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

        participantRepositoryMock.Setup(x => x.IsUserParticipatingInSettlement(
            It.IsAny<SettlementId>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        var request = new Fixture().Create<CreateExpenseCommand>();
            
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

        participantRepositoryMock.Setup(x => x.IsUserParticipatingInSettlement(
            It.IsAny<SettlementId>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(false);

        var request = new Fixture().Create<CreateExpenseCommand>();
            
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

        participantRepositoryMock.Setup(x => x.IsUserParticipatingInSettlement(
            It.IsAny<SettlementId>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        var request = new Fixture()
            .Build<CreateExpenseCommand>()
            .With(x => x.Title, string.Empty)
            .Create();

        var result = await createExpenseCommandHandler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseErrors.EmptyName);
    }
    
    
    [Fact]
    public async Task Handle_ShouldReturnNotFoundFailure_WhenNotAllParticipantsAreInTheSettlement()
    {
        participantRepositoryMock.Setup(x => x.AreAllParticipantsInSettlement(
            It.IsAny<SettlementId>(),
            It.IsAny<IEnumerable<ParticipantId>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(false);

        participantRepositoryMock.Setup(x => x.IsUserParticipatingInSettlement(
            It.IsAny<SettlementId>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        var request = new Fixture().Create<CreateExpenseCommand>();
            
        var result = await createExpenseCommandHandler.Handle(request, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ParticipantErrors.NotFound);
    }
}
