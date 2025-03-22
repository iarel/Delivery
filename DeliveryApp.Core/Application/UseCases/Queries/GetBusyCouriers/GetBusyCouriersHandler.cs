using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries;

public class GetBusyCouriersHandler : IRequestHandler<GetBusyCouriersCommand, GetCouriersResponse>
{
    private readonly ICourierRepository _courierRepository;

    public GetBusyCouriersHandler(ICourierRepository courierRepository)
    {
        _courierRepository = courierRepository;
    }

    public async Task<GetCouriersResponse> Handle(GetBusyCouriersCommand request, CancellationToken cancellationToken)
    {
        var allCouriers = await _courierRepository.GetAllAsync();

        var busyCouriers = allCouriers.Where(c => c.Status.Name == CourierStatus.Busy.Name).ToList();

        return new GetCouriersResponse(busyCouriers.Select(c => 
            new Courier
            {
                Id = c.Id,
                Name = c.Name,
                Location = new Location
                {
                    X = c.Location.X,
                    Y = c.Location.Y
                },
                TransportId = c.Transport.Id
            }).ToList());
    }
} 