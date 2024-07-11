using ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;
using ExpenseSplitter.Api.Presentation.Abstractions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public record GetAllSettlementsRequest(int Page, int PageSize);

public record GetAllSettlementsResponse(
    IEnumerable<GetAllSettlementsResponseSettlement> Settlements
);

public record GetAllSettlementsResponseSettlement(
    Guid Id,
    string Name
);

public class GetAllSettlementsEndpoint() : Endpoint<GetAllSettlementsRequest, GetAllSettlementsQuery, GetAllSettlementsQueryResult, GetAllSettlementsResponse>(
    Route: "{settlementId}",
    Group: EndpointGroup.Settlements,
    Method: EndpointMethod.Get,
    MapRequest: request => new (request.Page, request.PageSize),
    MapResponse: result => new GetAllSettlementsResponse(
        result.Settlements.Select(settlement => new GetAllSettlementsResponseSettlement(
            settlement.Id,
            settlement.Name
        ))
    ),
    ErrorStatusCodes: []
);
