using System;

namespace DeliveryApp.Core.Application.UseCases.Commands;

public class CreateBasketCommand
{
    public Guid BasketId { get; }

    public string Street { get; }
}