namespace DeliveryApp.Core.Application.UseCases.Queries;

public class GetActiveOrdersHandler
{
    public GetActiveOrdersResponse Handle(CancellationToken cancellationToken)
    {
        return new GetActiveOrdersResponse(new List<Order>());
    }
} 