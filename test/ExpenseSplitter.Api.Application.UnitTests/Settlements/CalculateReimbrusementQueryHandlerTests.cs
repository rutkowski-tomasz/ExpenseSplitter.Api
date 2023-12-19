using System.Runtime.InteropServices;
using ExpenseSplitter.Api.Application.Settlements.CalculateReimbrusement;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;
using ExpenseSplitter.Api.Domain.Shared;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

public class CalculateReimbrusementQueryHandlerTests
{
    private readonly CalculateReimbrusementQueryHandler handler;
    private readonly Fixture fixture;
    private readonly Mock<ISettlementUserRepository> settlementUserRepositoryMock;
    private readonly Mock<IExpenseRepository> expenseRepositoryMock;
    private readonly Mock<IParticipantRepository> participantRepositoryMock;

    public CalculateReimbrusementQueryHandlerTests()
    {
        fixture = CustomFixutre.Create();
        settlementUserRepositoryMock = new Mock<ISettlementUserRepository>();
        expenseRepositoryMock = new Mock<IExpenseRepository>();
        participantRepositoryMock = new Mock<IParticipantRepository>();

        settlementUserRepositoryMock
            .Setup(x => x.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        handler = new CalculateReimbrusementQueryHandler(
            settlementUserRepositoryMock.Object,
            expenseRepositoryMock.Object,
            participantRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldSuccess()
    {
        var query = fixture.Create<CalculateReimbrusementQuery>();

        var result = await handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserCantAccessGivenSettlement()
    {
        var query = fixture.Create<CalculateReimbrusementQuery>();

        settlementUserRepositoryMock
            .Setup(x => x.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await handler.Handle(query, default);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldHandleUnbalancedReimbrusement()
    {
        var alice = Participant.Create(SettlementId.New(), "Alice").Value;
        var bob = Participant.Create(SettlementId.New(), "Bob").Value;
        var participants = new List<Participant> { alice, bob };

        var expenses = new List<Expense>
        {
            GenerateExpense(alice.Id, 10, participants, new decimal[] { 8, 2 }),
            GenerateExpense(alice.Id, 10, participants, new decimal[] { 7, 3 }),
            GenerateExpense(bob.Id, 7, participants, new decimal[] { 6, 1 })
        };

        participantRepositoryMock
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(participants);
        
        expenseRepositoryMock
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expenses);
        
        var query = fixture.Create<CalculateReimbrusementQuery>();

        var result = await handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Balances.First().Should().BeEquivalentTo(new CalculateReimbrusementQueryResultBalance(
            alice.Id.Value,
            -1m
        ));
        result.Value.Balances.Skip(1).First().Should().BeEquivalentTo(new CalculateReimbrusementQueryResultBalance(
            bob.Id.Value,
            1m
        ));
        result.Value.SuggestedReimbrusements.First().Should().BeEquivalentTo(new CalculateReimbrusementQueryResultSuggestedReimbrusement(
            alice.Id.Value,
            bob.Id.Value,
            1m
        ));
    }

    [Fact]
    public async Task Handle_ShouldHandleUnbalancedReimbrusementForThreeParticipants()
    {
        var alice = Participant.Create(SettlementId.New(), "Alice").Value;
        var bob = Participant.Create(SettlementId.New(), "Bob").Value;
        var charlie = Participant.Create(SettlementId.New(), "Charlie").Value;
        var participants = new List<Participant> { alice, bob, charlie };

        var expenses = new List<Expense>
        {
            GenerateExpense(alice.Id, 20, participants, new decimal[] { 10, 5, 5 }),
            GenerateExpense(bob.Id, 15, participants, new decimal[] { 0, 10, 5 }),
            GenerateExpense(charlie.Id, 10, participants, new decimal[] { 5, 0, 5 }),
            GenerateExpense(alice.Id, 30, participants, new decimal[] { 10, 10, 10 }),
            GenerateExpense(bob.Id, 25, participants, new decimal[] { 5, 15, 5 }),
            GenerateExpense(charlie.Id, 20, participants, new decimal[] { 10, 5, 5 }),
            GenerateExpense(alice.Id, 35, participants, new decimal[] { 15, 10, 10 }),
            GenerateExpense(bob.Id, 40, participants, new decimal[] { 20, 10, 10 })
        };

        participantRepositoryMock
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(participants);
        
        expenseRepositoryMock
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expenses);
        
        var query = fixture.Create<CalculateReimbrusementQuery>();

        var result = await handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();

        result.Value.Should().BeEquivalentTo(new CalculateReimbrusementQueryResult(
            new List<CalculateReimbrusementQueryResultBalance>
            {
                new(alice.Id.Value, 10),
                new(bob.Id.Value, 15),
                new(charlie.Id.Value, -25),
            },
            new List<CalculateReimbrusementQueryResultSuggestedReimbrusement>
            {
                new(charlie.Id.Value, alice.Id.Value, 10),
                new(charlie.Id.Value, bob.Id.Value, 15),
            }
        ));
    }

    private Expense GenerateExpense(
        ParticipantId payingParticipantId,
        decimal value,
        List<Participant> participants,
        decimal[] values
    )
    {
        var expenseResult = Expense.Create(
            Guid.NewGuid().ToString(),
            Amount.Create(value).Value,
            DateOnly.FromDateTime(DateTime.UtcNow),
            SettlementId.New(),
            payingParticipantId
        );

        var expense = expenseResult.Value;

        participants.Should().HaveCount(values.Length);

        expense.Allocations = new List<Allocation>();
        for (var i = 0; i < participants.Count; i += 1)
        {
            var participant = participants[i];
            var allocationValue = values[i];

            expense.Allocations.Add(Allocation.Create(
                Amount.Create(allocationValue).Value,
                expenseResult.Value.Id,
                participant.Id
            ));
        }

        return expense;
    }
}