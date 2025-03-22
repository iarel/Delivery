using CSharpFunctionalExtensions;
using MediatR;
using Primitives;
using DeliveryApp.Core.Domain.Ports;

namespace DeliveryApp.Core.Application.UseCases.Commands;

public class MoveCouriersHandler : IRequestHandler<MoveCouriersCommand, UnitResult<Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    private readonly ICourierRepository _courierRepository;

    public MoveCouriersHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, ICourierRepository courierRepository)
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _courierRepository = courierRepository;
    }

    public async Task<UnitResult<Error>> Handle(MoveCouriersCommand command, CancellationToken cancellationToken)
    {
        var assignedOrders = await _orderRepository.GetAllAssigned();

        foreach (var order in assignedOrders)
        {
            if (order.CourierId == null)
                return UnitResult.Failure(GeneralErrors.ValueIsRequired(nameof(order.CourierId)));

            var courierResult = await _courierRepository.GetAsync(order.CourierId.Value);
            if (!courierResult.HasValue)
                return UnitResult.Failure(GeneralErrors.ValueIsRequired(nameof(order.CourierId)));

            var courier = courierResult.Value;

            var moveResult = courier.MoveTo(order.Location);
            if (moveResult.IsFailure)
                return UnitResult.Failure(moveResult.Error);

            if (order.Location == courier.Location)
            {
                order.Complete();
                courier.SetStatusToFree();
            }

            _courierRepository.Update(courier);
            _orderRepository.Update(order);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<Error>();
    }
}
