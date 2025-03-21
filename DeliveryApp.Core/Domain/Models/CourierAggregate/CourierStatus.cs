using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

public class CourierStatus : ValueObject
{
    public static CourierStatus Free => new(nameof(Free).ToLowerInvariant());
    public static CourierStatus Busy => new(nameof(Busy).ToLowerInvariant());

    private CourierStatus()
    {
    }

    private CourierStatus(string name) : this()
    {
        Name = name;
    }

    public string Name { get; private set;}

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}
