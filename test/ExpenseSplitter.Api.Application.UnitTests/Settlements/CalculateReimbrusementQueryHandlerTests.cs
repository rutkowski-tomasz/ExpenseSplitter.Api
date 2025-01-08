using ExpenseSplitter.Api.Application.Settlements.CalculateReimbursement;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

public class CalculateReimbursementQueryHandlerTests
{
    private readonly CalculateReimbursementQueryHandler handler;
    private readonly Fixture fixture = CustomFixture.Create();
    private readonly ISettlementUserRepository settlementUserRepository = Substitute.For<ISettlementUserRepository>();
    private readonly ISettlementRepository settlementRepository = Substitute.For<ISettlementRepository>();
    private readonly IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    private readonly Settlement settlement;

    public CalculateReimbursementQueryHandlerTests()
    {
        settlement = fixture.Create<Settlement>();
        settlement.AddParticipant(fixture.Create<string>());
        settlement.AddParticipant(fixture.Create<string>());

        settlementUserRepository
            .CanUserAccessSettlement(settlement.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        settlementRepository
            .GetById(settlement.Id, Arg.Any<CancellationToken>())
            .Returns(settlement);

        handler = new CalculateReimbursementQueryHandler(
            settlementUserRepository,
            settlementRepository,
            expenseRepository
        );
    }
    
    [Fact]
    public async Task Handle_ShouldSuccess()
    {
        var query = BuildQuery();

        var result = await handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserCantAccessGivenSettlement()
    {
        var query = BuildQuery();
    
        settlementUserRepository
            .CanUserAccessSettlement(settlement.Id, Arg.Any<CancellationToken>())
            .Returns(false);
    
        var result = await handler.Handle(query, default);
    
        result.IsFailure.Should().BeTrue();
    }
    
    [Fact]
    public async Task Handle_ShouldHandleUnbalancedReimbursement()
    {
        var a = settlement.Participants[0];
        var b = settlement.Participants[1];
        
        expenseRepository
            .GetAllBySettlementId(Arg.Any<SettlementId>(), Arg.Any<CancellationToken>())
            .Returns([
                GenerateExpense(a.Id, (a.Id, 8), (b.Id, 2) ),
                GenerateExpense(a.Id, (a.Id, 7), (b.Id, 3) ),
                GenerateExpense(b.Id, (a.Id, 6), (b.Id, 1) ),
            ]);
        
        var query = BuildQuery();
    
        var result = await handler.Handle(query, default);
    
        result.IsSuccess.Should().BeTrue();
        result.Value.Balances.First().Should().BeEquivalentTo(new CalculateReimbursementQueryResultBalance(
            a.Id.Value,
            -1m
        ));
        result.Value.Balances.Skip(1).First().Should().BeEquivalentTo(new CalculateReimbursementQueryResultBalance(
            b.Id.Value,
            1m
        ));
        result.Value.SuggestedReimbursements.First().Should().BeEquivalentTo(new CalculateReimbursementQueryResultSuggestedReimbursement(
            a.Id.Value,
            b.Id.Value,
            1m
        ));
    }
    
    [Fact]
    public async Task Handle_ShouldHandleUnbalancedReimbursementForThreeParticipants()
    {
        settlement.AddParticipant(fixture.Create<string>());
        
        var a = settlement.Participants[0];
        var b = settlement.Participants[1];
        var c = settlement.Participants[2];
    
        expenseRepository
            .GetAllBySettlementId(settlement.Id, Arg.Any<CancellationToken>())
            .Returns([
                GenerateExpense(a.Id, (a.Id, 10), (b.Id, 5), (c.Id, 5)),
                GenerateExpense(b.Id, (a.Id, 0), (b.Id, 10), (c.Id, 5)),
                GenerateExpense(c.Id, (a.Id, 5), (b.Id, 0), (c.Id, 5)),
                GenerateExpense(a.Id, (a.Id, 10), (b.Id, 10), (c.Id, 10)),
                GenerateExpense(b.Id, (a.Id, 5), (b.Id, 15), (c.Id, 5)),
                GenerateExpense(c.Id, (a.Id, 10), (b.Id, 5), (c.Id, 5)),
                GenerateExpense(a.Id, (a.Id, 15), (b.Id, 10), (c.Id, 10)),
                GenerateExpense(b.Id, (a.Id, 20), (b.Id, 10), (c.Id, 10))
            ]);

        var query = BuildQuery();
    
        var result = await handler.Handle(query, default);
    
        result.IsSuccess.Should().BeTrue();
    
        result.Value.Should().BeEquivalentTo(new CalculateReimbursementQueryResult(
            new List<CalculateReimbursementQueryResultBalance>
            {
                new(a.Id.Value, 10),
                new(b.Id.Value, 15),
                new(c.Id.Value, -25)
            },
            new List<CalculateReimbursementQueryResultSuggestedReimbursement>
            {
                new(c.Id.Value, a.Id.Value, 10),
                new(c.Id.Value, b.Id.Value, 15)
            }
        ));
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

    private CalculateReimbursementQuery BuildQuery()
    {
        return fixture
            .Build<CalculateReimbursementQuery>()
            .With(x => x.SettlementId, settlement.Id.Value)
            .Create();
    }
}
