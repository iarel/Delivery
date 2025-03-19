using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.OrderAggregate;

public class OrderTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var location = Location.Create(1, 1).Value;

        // Act
        var result = Order.Create(orderId, location);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var order = result.Value;
        order.Id.Should().Be(orderId);
        order.Location.Should().Be(location);
        order.Status.Should().Be(OrderStatus.Created);
        order.CourierId.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyId_ShouldReturnFailure()
    {
        // Arrange
        var location = Location.Create(1, 1).Value;

        // Act
        var result = Order.Create(Guid.Empty, location);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeneralErrors.ValueIsRequired("orderId"));
    }

    [Fact]
    public void Assign_WhenCreated_ShouldAssignCourier()
    {
        // Arrange
        var order = Order.Create(
            Guid.NewGuid(),
            Location.Create(1, 1).Value
        ).Value;
        var courier = Courier.Create(
            Guid.NewGuid(),
            "John Doe",
            Transport.Create(2, "Bicycle").Value,
            Location.Create(1, 1).Value
        ).Value;

        // Act
        var result = order.Assign(courier);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.CourierId.Should().Be(courier.Id);
        order.Status.Should().Be(OrderStatus.Assigned);
    }

    [Fact]
    public void Assign_WhenAlreadyAssigned_ShouldReturnFailure()
    {
        // Arrange
        var order = Order.Create(
            Guid.NewGuid(),
            Location.Create(1, 1).Value
        ).Value;
        var courier1 = Courier.Create(
            Guid.NewGuid(),
            "John Doe",
            Transport.Create(2, "Bicycle").Value,
            Location.Create(1, 1).Value
        ).Value;
        var courier2 = Courier.Create(
            Guid.NewGuid(),
            "Jane Doe",
            Transport.Create(2, "Bicycle").Value,
            Location.Create(1, 1).Value
        ).Value;

        order.Assign(courier1);

        // Act
        var result = order.Assign(courier2);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Order.Errors.CantAssignAlreadyAssignedOrder(courier2.Id));
    }

    [Fact]
    public void Assign_WithNullCourier_ShouldReturnFailure()
    {
        // Arrange
        var order = Order.Create(
            Guid.NewGuid(),
            Location.Create(1, 1).Value
        ).Value;

        // Act
        var result = order.Assign(null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeneralErrors.ValueIsRequired("courier"));
    }

    [Fact]
    public void Complete_WhenAssigned_ShouldCompleteOrder()
    {
        // Arrange
        var order = Order.Create(
            Guid.NewGuid(),
            Location.Create(1, 1).Value
        ).Value;
        var courier = Courier.Create(
            Guid.NewGuid(),
            "John Doe",
            Transport.Create(2, "Bicycle").Value,
            Location.Create(1, 1).Value
        ).Value;
        order.Assign(courier);

        // Act
        var result = order.Complete();

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public void Complete_WhenNotAssigned_ShouldReturnFailure()
    {
        // Arrange
        var order = Order.Create(
            Guid.NewGuid(),
            Location.Create(1, 1).Value
        ).Value;

        // Act
        var result = order.Complete();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Order.Errors.CantCompleteNotAssignedOrder());
    }
} 