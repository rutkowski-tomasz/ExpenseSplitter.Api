﻿using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.IntegrationTests;

public class SettlementTests : BaseIntegrationTest
{
    public SettlementTests(IntegrationTestWebAppFactory appFactory)
        : base(appFactory)
    {
    }

    [Fact]
    public async Task Create_ShouldAddNewSettlementToDatabase()
    {
        var command = new Fixture().Create<CreateSettlementCommand>();

        var result = await Sender.Send(command);

        result.Value.SettlementId.Should().NotBeEmpty();

        DbContext
            .Set<Settlement>()
            .FirstOrDefault(x => x.Id == new SettlementId(result.Value.SettlementId))
            .Should()
            .NotBeNull();
    }
}
