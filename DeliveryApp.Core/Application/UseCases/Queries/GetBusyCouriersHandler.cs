namespace DeliveryApp.Core.Application.UseCases.Queries;

public class GetBusyCouriersHandler
{
    public GetCouriersResponse Handle(CancellationToken cancellationToken)
    {
        return new GetCouriersResponse(new List<Courier>());
    }
} 