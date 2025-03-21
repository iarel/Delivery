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

        if (order.Status != OrderStatus.Created)
            return Order.Errors.CantAssignNotCreatedOrder();


        var courier = couriers.Where(c => c.Status == CourierStatus.Free);

        var courierWinner = TakeWithHighestScore(courier, order);

        if (courierWinner == null) return GeneralErrors.ValueIsRequired("courier");

        return Result.Success<Order, Error>(order);
    }
    
    private Courier TakeWithHighestScore(IEnumerable<Courier> couriers, Order order)
    {
        var courierScores = new Dictionary<double, Courier>();
        foreach (var courier in couriers)
        {
            var timeToLocation = courier.CalculateTimeToLocation(order.Location);
            if (timeToLocation.IsSuccess)
            {
                courierScores.Add(timeToLocation.Value, courier);
            }
        }

        return courierScores.OrderBy(x => x.Key).First().Value;
    }
}
