using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries;

public class GetActiveOrdersHandler : IRequestHandler<GetActiveOrdersCommand, GetActiveOrdersResponse>
{
    private readonly IOrderRepository _orderRepository;

    public GetActiveOrdersHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<GetActiveOrdersResponse> Handle(GetActiveOrdersCommand request, CancellationToken cancellationToken)
    {
        var assignedOrders = await _orderRepository.GetAllAssigned();
        
        var orders = assignedOrders.Select(o => 
            new Order
            {
                Id = o.Id,
                Location = new()
                {
                    X = o.Location.X,
                    Y = o.Location.Y
                }
            }).ToList();
        return new GetActiveOrdersResponse(orders);
    }
} 