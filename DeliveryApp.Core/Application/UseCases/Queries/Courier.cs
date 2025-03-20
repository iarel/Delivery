namespace DeliveryApp.Core.Application.UseCases.Queries;

public class Courier
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public Location Location { get; set; }

    public int TransportId { get; set; }
} 