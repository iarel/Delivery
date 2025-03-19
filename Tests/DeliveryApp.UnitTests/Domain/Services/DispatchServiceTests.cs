using System;
using System.Collections.Generic;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Services;

public class DispatchServiceTests
{
    private readonly DispatchService _dispatchService;

    public DispatchServiceTests()
    {
        _dispatchService = new DispatchService();
    }

    [Fact]
    public void Dispatch_WithValidOrderAndCouriers_ShouldReturnOrder()
    {
        // Arrange
        var order = Order.Create(
            Guid.NewGuid(),
            Location.Create(3, 3).Value
        ).Value;

        var couriers = new List<Courier>
        {
            Courier.Create(
                Guid.NewGuid(),
                "Slow Courier",
                Transport.Create(1, "Walking").Value,
                Location.Create(1, 1).Value
            ).Value,
            Courier.Create(
                Guid.NewGuid(),
                "Fast Courier",
                Transport.Create(3, "Scooter").Value,
                Location.Create(1, 1).Value
            ).Value
        };

        // Act
        var result = _dispatchService.Dispatch(order, couriers);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(order);
    }

    [Fact]
    public void Dispatch_WithNullOrder_ShouldReturnFailure()
    {
        // Arrange
        Order order = null;
        var couriers = new List<Courier>
        {
            Courier.Create(
                Guid.NewGuid(),
                "Courier",
                Transport.Create(2, "Bicycle").Value,
                Location.Create(1, 1).Value
            ).Value
        };

        // Act
        var result = _dispatchService.Dispatch(order, couriers);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeneralErrors.ValueIsRequired("order"));
    }

    [Fact]
    public void Dispatch_WithNullCouriers_ShouldReturnFailure()
    {
        // Arrange
        var order = Order.Create(
            Guid.NewGuid(),
            Location.Create(3, 3).Value
        ).Value;
        IEnumerable<Courier> couriers = null;

        // Act
        var result = _dispatchService.Dispatch(order, couriers);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeneralErrors.ValueIsRequired("couriers"));
    }

    [Fact]
    public void Dispatch_WithNotCreatedOrder_ShouldReturnFailure()
    {
        // Arrange
        var order = Order.Create(
            Guid.NewGuid(),
            Location.Create(3, 3).Value
        ).Value;
        var courier = Courier.Create(
            Guid.NewGuid(),
            "Courier",
            Transport.Create(2, "Bicycle").Value,
            Location.Create(1, 1).Value
        ).Value;
        order.Assign(courier); // Assign to make it not Created

        var couriers = new List<Courier> { courier };

        // Act
        var result = _dispatchService.Dispatch(order, couriers);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Order.Errors.CantAssignNotCreatedOrder());
    }

    [Fact]
    public void Dispatch_WithNoFreeCouriers_ShouldThrowException()
    {
        // Arrange
        var order = Order.Create(
            Guid.NewGuid(),
            Location.Create(3, 3).Value
        ).Value;

        var courier = Courier.Create(
            Guid.NewGuid(),
            "Busy Courier",
            Transport.Create(2, "Bicycle").Value,
            Location.Create(1, 1).Value
        ).Value;
        courier.SetStatusToBusy();
        var couriers = new List<Courier> { courier };

        // Act & Assert
        Action act = () => _dispatchService.Dispatch(order, couriers);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Sequence contains no elements");
    }

    [Fact]
    public void Dispatch_WithEmptyCouriers_ShouldThrowException()
    {
        // Arrange
        var order = Order.Create(
            Guid.NewGuid(),
            Location.Create(3, 3).Value
        ).Value;
        var couriers = new List<Courier>();

        // Act & Assert
        Action act = () => _dispatchService.Dispatch(order, couriers);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Sequence contains no elements");
    }
} 