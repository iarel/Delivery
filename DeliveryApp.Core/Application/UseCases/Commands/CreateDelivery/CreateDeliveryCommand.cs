using System;

namespace DeliveryApp.Core.Application.UseCases.Commands;

public class CreateDeliveryCommand
{
    public Guid BasketId { get; }

    public string Street { get; }
}