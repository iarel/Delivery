using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries;

public class GetActiveOrdersCommand : IRequest<GetActiveOrdersResponse>
{
}
