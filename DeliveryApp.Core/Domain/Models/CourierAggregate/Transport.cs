using CSharpFunctionalExtensions;

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
}
