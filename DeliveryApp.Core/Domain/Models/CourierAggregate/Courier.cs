using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;
using static DeliveryApp.Core.Domain.Model.OrderAggregate.Order;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

public class Courier : Aggregate
{
    public string Name { get; private set; }
    public Transport Transport { get; private set; }
    public Location Location { get; private set; }
    public CourierStatus Status { get; private set; }

    private Courier()
    {
    }

    private Courier(Guid id, string name, Transport transport, Location location)
    {
        Id = id;
        Name = name;
        Transport = transport;
        Location = location;
        Status = CourierStatus.Free;
    }

    public Result<Courier, Error> SetStatusToBusy()
    {
        if (Status == CourierStatus.Busy) return GeneralErrors.ValueIsRequired(nameof(Status));
        Status = CourierStatus.Busy;
        return this;
    }

    public Result<Courier, Error> SetStatusToFree()
    {
        if (Status == CourierStatus.Free) return GeneralErrors.ValueIsRequired(nameof(Status));
        Status = CourierStatus.Free;
        return this;
    }

    public UnitResult<Error> MoveTo(Location location)
    {
        if (Status == CourierStatus.Busy) return GeneralErrors.CantMoveBusyCourier();
        
        var newLocation = Transport.Move(Location, location);
        if (newLocation.IsFailure) return newLocation.Error;

        Location = newLocation.Value;
        return UnitResult.Success<Error>();
    }

    public Result<double, Error> CalculateTimeToLocation(Location location)
    {
        if (Status == CourierStatus.Busy) return GeneralErrors.CantMoveBusyCourier();

        var distance = Location.CalculateDistanceTo(location);

        if (distance.IsFailure) return distance.Error;
        
        return Result.Success<double, Error>(distance.Value / Transport.Speed);
    }

    public static Result<Courier, Error> Create(Guid id, string name, Transport transport, Location location)
    {
        if (id == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(id));
        if (string.IsNullOrWhiteSpace(name)) return GeneralErrors.ValueIsRequired(nameof(name));
        if (transport == null) return GeneralErrors.ValueIsRequired(nameof(transport));
        if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));

        return new Courier(id, name, transport, location);
    }
}
