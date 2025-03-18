using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.SharedKernel;

public class Location : ValueObject
{
    public int X { get; }
    public int Y { get; }

    public static Location MaxLocation = new Location(10, 10);

    public static Location MinLocation = new Location(1, 1);

    private static readonly Random _random = new Random();
    
    public static Location RandomLocation()
    {
        return new Location(_random.Next(1, 11), _random.Next(1, 11));
    }

    public int CalculateDistanceTo(Location other)
    {
        var horizontalDistance = Math.Abs(X - other.X);
        var verticalDistance = Math.Abs(Y - other.Y);
        
        return horizontalDistance + verticalDistance;
    }

    private Location(int x, int y)
    {
        X = x;
        Y = y;
    }

    private Location() {}

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }

    public static Result<Location> Create(int x, int y)
    {
        if (x < 1 || x > 10)
            return Result.Failure<Location>("X must be between 1 and 10");

        if (y < 1 || y > 10) 
            return Result.Failure<Location>("Y must be between 1 and 10");

        return Result.Success(new Location(x, y));
    }
}