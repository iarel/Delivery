using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services;

public class DispatchService : IDispatchService
{
    public Result<Order, Error> Dispatch(Order order, IEnumerable<Courier> couriers)
    {
        if (order == null) return GeneralErrors.ValueIsRequired("order");
        if (couriers == null) return GeneralErrors.ValueIsRequired("couriers");
        if (order.Status != OrderStatus.Created) return Order.Errors.CantAssignNotCreatedOrder();

        var courierWithHighestScore = TakeWithHighestScore(couriers, order);
        if (courierWithHighestScore == null) return GeneralErrors.ValueIsRequired("courier");

        var assignResult = order.Assign(courierWithHighestScore);
        if (assignResult.IsFailure) return assignResult.Error;

        return Result.Success<Order, Error>(order);
    }
    
    private Courier TakeWithHighestScore(IEnumerable<Courier> couriers, Order order)
        => couriers
            .Where(c => c.Status == CourierStatus.Free)
            .Select(courier => (courier, timeToLocation: courier.CalculateTimeToLocation(order.Location)))
            .Where(tuple => tuple.timeToLocation.IsSuccess)
            .MaxBy(tuple => tuple.timeToLocation.Value).courier;
}
