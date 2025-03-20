
namespace DeliveryApp.Core.Application.UseCases.Queries;

public class GetActiveOrdersResponse
{
    public GetActiveOrdersResponse(List<Order> orders)
    {
        Orders.AddRange(orders);
    }

    public List<Order> Orders { get; set; } = new();
}