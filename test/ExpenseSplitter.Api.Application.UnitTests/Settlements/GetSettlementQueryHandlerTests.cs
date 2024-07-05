﻿using ExpenseSplitter.Api.Application.Settlements.GetSettlement;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;
using ExpenseSplitter.Api.Domain.Shared;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;
public class GetSettlementQueryHandlerTests
{
    private readonly Fixture fixture;
    private readonly Mock<ISettlementRepository> settlementRepositoryMock;
    private readonly Mock<ISettlementUserRepository> settlementUserRepositoryMock;
    private readonly Mock<IExpenseRepository> expenseRepositoryMock;
    private readonly GetSettlementQueryHandler handler;

    public GetSettlementQueryHandlerTests()
    {
        fixture = CustomFixutre.Create();
        settlementRepositoryMock = new Mock<ISettlementRepository>();
        settlementUserRepositoryMock = new Mock<ISettlementUserRepository>();
        var participantRepositoryMock = new Mock<IParticipantRepository>();
        expenseRepositoryMock = new Mock<IExpenseRepository>();

        handler = new GetSettlementQueryHandler(
            settlementRepositoryMock.Object,
            settlementUserRepositoryMock.Object,
            participantRepositoryMock.Object,
            expenseRepositoryMock.Object
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
        response.Error.Type.Should().Be(SettlementErrors.NotFound.Type);
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

        expenseRepositoryMock
            .Setup(x => x.GetAllBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Expense>()
            {
                GenerateExpense(alice.Id, 10, participants, new decimal[] { 8, 2 }),
                GenerateExpense(alice.Id, 10, participants, new decimal[] { 7, 3 }),
                GenerateExpense(bob.Id, 7, participants, new decimal[] { 6, 1 })
            });

        var query = fixture.Create<GetSettlementQuery>();

        var response = await handler.Handle(query, default);

        response.IsSuccess.Should().BeTrue();

        response.Value.TotalCost.Should().Be(27);
        response.Value.YourCost.Should().Be(21);
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

