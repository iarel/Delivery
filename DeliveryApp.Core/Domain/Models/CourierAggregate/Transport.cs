using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Models.CourierAggregate;

public class Transport : Entity<Guid>
{
    private const int _minSpeed = 1;

    private const int _maxSpeed = 3;

    public int Speed { get; private set; }
 
    public string Name { get; private set; }

    private Transport(int speed, string name)
    {
        Id = Guid.NewGuid();
        Speed = speed;
        Name = name;
    }

    public static Result<Transport> Create(int speed, string name)
    {
        if (speed < _minSpeed || speed > _maxSpeed)
            return Result.Failure<Transport>($"Speed must be between {_minSpeed} and {_maxSpeed}");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Transport>("Name must be provided");

        var transport = new Transport(speed, name);
        return Result.Success(transport);
    }

        public Result<Location, Error> Move(Location current, Location target)
    {
        if (current == null) return GeneralErrors.ValueIsRequired(nameof(current));
        if (target == null) return GeneralErrors.ValueIsRequired(nameof(target));

        var difX = target.X - current.X;
        var difY = target.Y - current.Y;
        var cruisingRange = Speed;

        var moveX = Math.Clamp(difX, -cruisingRange, cruisingRange);
        cruisingRange -= Math.Abs(moveX);

        var moveY = Math.Clamp(difY, -cruisingRange, cruisingRange);

        var newLocation = Location.Create(current.X + moveX, current.Y + moveY).Value;
        return newLocation;
    }

}
