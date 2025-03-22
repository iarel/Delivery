using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Ports;
using MediatR;
using Primitives;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;

namespace DeliveryApp.Core.Application.UseCases.Commands;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, UnitResult<Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;

    public CreateOrderHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository)
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
    }

    public async Task<UnitResult<Error>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var location = Location.RandomLocation();

        var order = Order.Create(command.BasketId, location);
        
        if (order.IsFailure)
            return UnitResult.Failure<Error>(order.Error);

        await _orderRepository.AddAsync(order.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }
}
