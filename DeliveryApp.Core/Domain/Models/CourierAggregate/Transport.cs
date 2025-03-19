using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Models.CourierAggregate;

public class Transport : Entity<Guid>
{
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
        if (speed < 1 || speed > 3)
            return Result.Failure<Transport>("Speed must be between 1 and 3");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Transport>("Name must be provided");

        var transport = new Transport(speed, name);
        return Result.Success(transport);
    }
}
