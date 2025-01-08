using ExpenseSplitter.Api.Application.Abstractions.Etag;
using ExpenseSplitter.Api.Application.Settlements.GetSettlement;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;
public class GetSettlementQueryHandlerTests
{
    private readonly Fixture fixture = CustomFixture.Create();
    private readonly ISettlementRepository settlementRepository = Substitute.For<ISettlementRepository>();
    private readonly ISettlementUserRepository settlementUserRepository = Substitute.For<ISettlementUserRepository>();
    private readonly IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    private readonly IEtagService etagService = Substitute.For<IEtagService>();
    private readonly GetSettlementQueryHandler handler;
    private readonly Settlement settlement;
    private readonly SettlementUser settlementUser;

    public GetSettlementQueryHandlerTests()
    {
        settlement = fixture.Create<Settlement>();
        settlement.AddParticipant(fixture.Create<string>());
        settlement.AddParticipant(fixture.Create<string>());
        
        settlementRepository
            .GetById(settlement.Id, Arg.Any<CancellationToken>())
            .Returns(settlement);

        settlementUser = fixture.Create<SettlementUser>();
        settlementUser.SetParticipantId(settlement.Participants[0].Id);

        settlementUserRepository
            .GetBySettlementId(settlement.Id, Arg.Any<CancellationToken>())
            .Returns(settlementUser);

        handler = new GetSettlementQueryHandler(
            settlementRepository,
            settlementUserRepository,
            expenseRepository,
            etagService
        );
    }
    
    [Fact]
    public async Task Handle_ShouldMapSettlementsToDto()
    {
        var query = BuildQuery();

        var response = await handler.Handle(query, default);

        response.IsSuccess.Should().BeTrue();
        response.Value.Name.Should().Be(settlement.Name);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSettlementWithTheIdDoesntExist()
    {
        settlementRepository
            .GetById(settlement.Id, Arg.Any<CancellationToken>())
            .Returns((Settlement) default);
        
        var query = BuildQuery();

        var response = await handler.Handle(query, default);

        response.IsFailure.Should().BeTrue();
        response.AppError.Type.Should().Be(SettlementErrors.NotFound.Type);
    }

    [Fact]
    public async Task Handle_ShouldCorrectlyCalculateTotalAndUserCost()
    {
        var a = settlement.Participants[0];
        var b = settlement.Participants[1];

        expenseRepository
            .GetAllBySettlementId(settlement.Id, Arg.Any<CancellationToken>())
            .Returns([
                GenerateExpense(a.Id, (a.Id, 8), (b.Id, 2)),
                GenerateExpense(a.Id, (a.Id, 7), (b.Id, 3)),
                GenerateExpense(a.Id, (a.Id, 6), (b.Id, 1)),
            ]);

        var query = BuildQuery();

        var response = await handler.Handle(query, default);

        response.IsSuccess.Should().BeTrue();

        response.Value.TotalCost.Should().Be(27);
        response.Value.YourCost.Should().Be(21);
    }

    private GetSettlementQuery BuildQuery()
    {
        return fixture
            .Build<GetSettlementQuery>()
            .With(x => x.SettlementId, settlement.Id.Value)
            .Create();
    }

    private Expense GenerateExpense(
        ParticipantId payingParticipantId,
        params (ParticipantId ParticipantId, decimal Amount)[] expenseAllocations
    )
    {
        var expenseResult = Expense.Create(
            fixture.Create<string>(),
            fixture.Create<DateOnly>(),
            settlement.Id,
            payingParticipantId,
            expenseAllocations.ToDictionary(x => x.ParticipantId, x => x.Amount)
        );

        return expenseResult.Value;
    }
}

