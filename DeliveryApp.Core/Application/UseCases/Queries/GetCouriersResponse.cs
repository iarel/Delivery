
namespace DeliveryApp.Core.Application.UseCases.Queries;

public class GetCouriersResponse
{
    public GetCouriersResponse(List<Courier> couriers)
    {
        Couriers.AddRange(couriers);
    }

    public List<Courier> Couriers { get; set; } = new();
}