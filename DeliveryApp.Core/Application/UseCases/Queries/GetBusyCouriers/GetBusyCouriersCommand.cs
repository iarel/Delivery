using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries;

public class GetBusyCouriersCommand : IRequest<GetCouriersResponse>
{
}
