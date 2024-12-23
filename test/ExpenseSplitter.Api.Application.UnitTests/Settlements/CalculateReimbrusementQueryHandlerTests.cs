using ExpenseSplitter.Api.Application.Settlements.CalculateReimbursement;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Common;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

public class CalculateReimbursementQueryHandlerTests
{
    private readonly CalculateReimbursementQueryHandler handler;
    private readonly Fixture fixture;
    private readonly Mock<ISettlementUserRepository> settlementUserRepositoryMock;
    private readonly Mock<IExpenseRepository> expenseRepositoryMock;
    private readonly Mock<IParticipantRepository> participantRepositoryMock;

    public CalculateReimbursementQueryHandlerTests()
    {
        fixture = CustomFixture.Create();
        settlementUserRepositoryMock = new Mock<ISettlementUserRepository>();
        expenseRepositoryMock = new Mock<IExpenseRepository>();
        participantRepositoryMock = new Mock<IParticipantRepository>();

        settlementUserRepositoryMock
            .Setup(x => x.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        participantRepositoryMock
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        handler = new CalculateReimbursementQueryHandler(
            settlementUserRepositoryMock.Object,
            expenseRepositoryMock.Object,
            participantRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldSuccess()
    {
        var query = fixture.Create<CalculateReimbursementQuery>();

        var result = await handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserCantAccessGivenSettlement()
    {
        var query = fixture.Create<CalculateReimbursementQuery>();

        settlementUserRepositoryMock
            .Setup(x => x.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await handler.Handle(query, default);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldHandleUnbalancedReimbursement()
    {
        var alice = Participant.Create(SettlementId.New(), "Alice").Value;
        var bob = Participant.Create(SettlementId.New(), "Bob").Value;
        var participants = new List<Participant> { alice, bob };

        var expenses = new List<Expense>
        {
            GenerateExpense(alice.Id, 10, participants, [8, 2]),
            GenerateExpense(alice.Id, 10, participants, [7, 3]),
            GenerateExpense(bob.Id, 7, participants, [6, 1])
        };

        participantRepositoryMock
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(participants);
        
        expenseRepositoryMock
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expenses);
        
        var query = fixture.Create<CalculateReimbursementQuery>();

        var result = await handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Balances.First().Should().BeEquivalentTo(new CalculateReimbursementQueryResultBalance(
            alice.Id.Value,
            -1m
        ));
        result.Value.Balances.Skip(1).First().Should().BeEquivalentTo(new CalculateReimbursementQueryResultBalance(
            bob.Id.Value,
            1m
        ));
        result.Value.SuggestedReimbursements.First().Should().BeEquivalentTo(new CalculateReimbursementQueryResultSuggestedReimbursement(
            alice.Id.Value,
            bob.Id.Value,
            1m
        ));
    }

    [Fact]
    public async Task Handle_ShouldHandleUnbalancedReimbursementForThreeParticipants()
    {
        var alice = Participant.Create(SettlementId.New(), "Alice").Value;
        var bob = Participant.Create(SettlementId.New(), "Bob").Value;
        var charlie = Participant.Create(SettlementId.New(), "Charlie").Value;
        var participants = new List<Participant> { alice, bob, charlie };

        var expenses = new List<Expense>
        {
            GenerateExpense(alice.Id, 20, participants, [10, 5, 5]),
            GenerateExpense(bob.Id, 15, participants, [0, 10, 5]),
            GenerateExpense(charlie.Id, 10, participants, [5, 0, 5]),
            GenerateExpense(alice.Id, 30, participants, [10, 10, 10]),
            GenerateExpense(bob.Id, 25, participants, [5, 15, 5]),
            GenerateExpense(charlie.Id, 20, participants, [10, 5, 5]),
            GenerateExpense(alice.Id, 35, participants, [15, 10, 10]),
            GenerateExpense(bob.Id, 40, participants, [20, 10, 10])
        };

        participantRepositoryMock
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(participants);
        
        expenseRepositoryMock
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expenses);
        
        var query = fixture.Create<CalculateReimbursementQuery>();

        var result = await handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();

        result.Value.Should().BeEquivalentTo(new CalculateReimbursementQueryResult(
            new List<CalculateReimbursementQueryResultBalance>
            {
                new(alice.Id.Value, 10),
                new(bob.Id.Value, 15),
                new(charlie.Id.Value, -25)
            },
            new List<CalculateReimbursementQueryResultSuggestedReimbursement>
            {
                new(charlie.Id.Value, alice.Id.Value, 10),
                new(charlie.Id.Value, bob.Id.Value, 15)
            }
        ));
    }

    private static Expense GenerateExpense(
        ParticipantId payingParticipantId,
        decimal value,
        List<Participant> participants,
        List<decimal> values
    )
    {
        var expenseResult = Expense.Create(
            Guid.CreateVersion7().ToString(),
            Amount.Create(value).Value,
            DateOnly.FromDateTime(DateTime.UtcNow),
            SettlementId.New(),
            payingParticipantId
        );

        var expense = expenseResult.Value;

        participants.Should().HaveCount(values.Count);

        expense.Allocations = [];
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
