using AutoFixture.Dsl;
using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Expenses.CreateExpense;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Expenses;

public class CreateExpenseCommandHandlerTests
{
    private readonly CreateExpenseCommandHandler createExpenseCommandHandler;
    private readonly ISettlementUserRepository settlementUserRepository = Substitute.For<ISettlementUserRepository>();
    private readonly ISettlementRepository settlementRepository = Substitute.For<ISettlementRepository>();
    private readonly IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    private readonly IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly Fixture fixture = CustomFixture.Create();
    private readonly Settlement settlement;
    private readonly CreateExpenseCommand command;

    public CreateExpenseCommandHandlerTests()
    {
        createExpenseCommandHandler = new CreateExpenseCommandHandler(
            settlementUserRepository,
            expenseRepository,
            settlementRepository,
            dateTimeProvider,
            unitOfWork
        );

        settlement = fixture.Create<Settlement>();
        settlement.AddParticipant(fixture.Create<string>());
        settlement.AddParticipant(fixture.Create<string>());

        var participantIds = settlement.Participants.Select(x => x.Id.Value).ToList();
        command = ComposeCommand(participantIds).Create();

        settlementUserRepository
            .CanUserAccessSettlement(settlement.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        settlementRepository
            .GetById(settlement.Id, Arg.Any<CancellationToken>())
            .Returns(settlement);
    }

    [Fact]
    public async Task Handle_ShouldProcess()
    {
        var result = await createExpenseCommandHandler.Handle(command, default);
        
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnForbiddenFailure_WhenUserIsNotParticipatingInSettlement()
    {
        settlementUserRepository
            .CanUserAccessSettlement(settlement.Id, Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await createExpenseCommandHandler.Handle(command, default);
    
        result.IsFailure.Should().BeTrue();
        result.AppError.Should().Be(SettlementErrors.Forbidden);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnEmptyNameFailure_WhenExpenseTitleIsEmpty()
    {
        var participantIds = settlement.Participants.Select(x => x.Id.Value).ToList();
        var emptyTitleCommand = ComposeCommand(participantIds)
            .With(x => x.Title, string.Empty)
            .Create();
    
        var result = await createExpenseCommandHandler.Handle(emptyTitleCommand, default);
    
        result.IsFailure.Should().BeTrue();
        result.AppError.Should().Be(ExpenseErrors.EmptyTitle);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnNotFoundFailure_WhenNotAllParticipantsAreInTheSettlement()
    {
        var participantIds = settlement.Participants.Select(x => x.Id.Value).ToList();
        participantIds.Add(Guid.NewGuid());
        
        var notExistingParticipantCommand = ComposeCommand(participantIds).Create();
            
        var result = await createExpenseCommandHandler.Handle(notExistingParticipantCommand, default);
    
        result.IsFailure.Should().BeTrue();
        result.AppError.Should().Be(ParticipantErrors.NotFound);
    }
    
    private IPostprocessComposer<CreateExpenseCommand> ComposeCommand(List<Guid> participantIds)
    {
        var allocations = participantIds
            .Select(x => fixture
                .Build<CreateExpenseCommandAllocation>()
                .With(y => y.ParticipantId, x)
                .Create()
            );

        return fixture
            .Build<CreateExpenseCommand>()
            .With(x => x.SettlementId, settlement.Id.Value)
            .With(x => x.Allocations, allocations)
            .With(x => x.PayingParticipantId, participantIds[0]);
    }
}
