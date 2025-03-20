using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.SharedKernel;

public class Location : ValueObject
{
    private const int _minLocation = 1;
    private const int _maxLocation = 10;

    public int X { get; }
    public int Y { get; }

    public static Location MaxLocation = new Location(_maxLocation, _maxLocation);

    public static Location MinLocation = new Location(_minLocation, _minLocation);

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
        if (x < _minLocation || x > _maxLocation)
            return Result.Failure<Location>("X must be between 1 and 10");

        if (y < _minLocation || y > _maxLocation)
            return Result.Failure<Location>("Y must be between 1 and 10");

        return Result.Success(new Location(x, y));
    }
}