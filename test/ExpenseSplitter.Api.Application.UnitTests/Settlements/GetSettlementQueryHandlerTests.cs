using ExpenseSplitter.Api.Application.Abstractions.Etag;
using ExpenseSplitter.Api.Application.Settlements.GetSettlement;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Common;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;
public class GetSettlementQueryHandlerTests
{
    private readonly Fixture fixture;
    private readonly Mock<ISettlementRepository> settlementRepositoryMock;
    private readonly Mock<ISettlementUserRepository> settlementUserRepositoryMock;
    private readonly Mock<IExpenseRepository> expenseRepositoryMock;
    private readonly GetSettlementQueryHandler handler;
    private readonly Mock<IParticipantRepository> participantRepositoryMock;

    public GetSettlementQueryHandlerTests()
    {
        fixture = CustomFixture.Create();
        settlementRepositoryMock = new Mock<ISettlementRepository>();
        settlementUserRepositoryMock = new Mock<ISettlementUserRepository>();
        participantRepositoryMock = new Mock<IParticipantRepository>();
        expenseRepositoryMock = new Mock<IExpenseRepository>();
        var etagServiceMock = new Mock<IEtagService>();

        handler = new GetSettlementQueryHandler(
            settlementRepositoryMock.Object,
            settlementUserRepositoryMock.Object,
            participantRepositoryMock.Object,
            expenseRepositoryMock.Object,
            etagServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldMapSettlementsToDto()
    {
        var settlement = fixture.Create<Settlement>();

        settlementRepositoryMock
            .Setup(x => x.GetById(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(settlement);

        settlementUserRepositoryMock
            .Setup(x => x.GetBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.Create<SettlementUser>());

        participantRepositoryMock
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.CreateMany<Participant>().ToList());

        var query = fixture.Create<GetSettlementQuery>();

        var response = await handler.Handle(query, default);

        response.IsSuccess.Should().BeTrue();
        response.Value.Name.Should().Be(settlement.Name);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSettlementWithTheIdDoesntExist()
    {
        var query = fixture.Create<GetSettlementQuery>();

        settlementUserRepositoryMock
            .Setup(x => x.GetBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.Create<SettlementUser>());

        var response = await handler.Handle(query, default);

        response.IsFailure.Should().BeTrue();
        response.AppError.Type.Should().Be(SettlementErrors.NotFound.Type);
    }

    [Fact]
    public async Task Handle_ShouldCorrectlyCalculateTotalAndUserCost()
    {
        var alice = Participant.Create(SettlementId.New(), "Alice").Value;
        var bob = Participant.Create(SettlementId.New(), "Bob").Value;
        var participants = new List<Participant> { alice, bob };

        settlementRepositoryMock
            .Setup(x => x.GetById(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.Create<Settlement>());

        var settlementUser = fixture.Create<SettlementUser>();
        settlementUser.SetParticipantId(alice.Id);
        settlementUserRepositoryMock
            .Setup(x => x.GetBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(settlementUser);

        participantRepositoryMock
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(participants);
        
        expenseRepositoryMock
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Expense>
            {
                GenerateExpense(alice.Id, 10, participants, [8, 2]),
                GenerateExpense(alice.Id, 10, participants, [7, 3]),
                GenerateExpense(bob.Id, 7, participants, [6, 1])
            });

        var query = fixture.Create<GetSettlementQuery>();

        var response = await handler.Handle(query, default);

        response.IsSuccess.Should().BeTrue();

        response.Value.TotalCost.Should().Be(27);
        response.Value.YourCost.Should().Be(21);
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

