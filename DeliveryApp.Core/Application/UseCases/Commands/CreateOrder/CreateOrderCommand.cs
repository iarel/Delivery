using System;
using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands;

public class CreateOrderCommand : IRequest<UnitResult<Error>>
{
    public CreateOrderCommand(Guid basketId, string street)
    {
        if (basketId == Guid.Empty)
            throw new ArgumentException("BasketId cannot be empty", nameof(basketId));

        if (string.IsNullOrEmpty(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));

        BasketId = basketId;
        Street = street;
    }

    public Guid BasketId { get; private set; }

    public string Street { get; private set; }
}