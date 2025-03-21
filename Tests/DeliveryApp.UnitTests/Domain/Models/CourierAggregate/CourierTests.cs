using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.CourierAggregate;

public class CourierTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateCourier()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John Doe";
        var transport = Transport.Create(2, "Bicycle").Value;
        var location = Location.Create(1, 1).Value;

        // Act
        var result = Courier.Create(id, name, transport, location);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var courier = result.Value;
        courier.Id.Should().Be(id);
        courier.Name.Should().Be(name);
        courier.Transport.Should().Be(transport);
        courier.Location.Should().Be(location);
        courier.Status.Should().Be(CourierStatus.Free);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidName_ShouldReturnFailure(string invalidName)
    {
        // Arrange
        var id = Guid.NewGuid();
        var transport = Transport.Create(2, "Bicycle").Value;
        var location = Location.Create(1, 1).Value;

        // Act
        var result = Courier.Create(id, invalidName, transport, location);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeneralErrors.ValueIsRequired("name"));
    }

    [Fact]
    public void SetStatusToBusy_WhenFree_ShouldChangeStatus()
    {
        // Arrange
        var courier = Courier.Create(
            Guid.NewGuid(),
            "John Doe",
            Transport.Create(2, "Bicycle").Value,
            Location.Create(1, 1).Value
        ).Value;

        // Act
        var result = courier.SetStatusToBusy();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(CourierStatus.Busy);
    }

    [Fact]
    public void SetStatusToBusy_WhenAlreadyBusy_ShouldReturnFailure()
    {
        // Arrange
        var courier = Courier.Create(
            Guid.NewGuid(),
            "John Doe",
            Transport.Create(2, "Bicycle").Value,
            Location.Create(1, 1).Value
        ).Value;
        courier.SetStatusToBusy();

        // Act
        var result = courier.SetStatusToBusy();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeneralErrors.ValueIsRequired(nameof(courier.Status)));
    }

    [Fact]
    public void MoveTo_WhenFree_ShouldUpdateLocation()
    {
        // Arrange
        var courier = Courier.Create(
            Guid.NewGuid(),
            "John Doe",
            Transport.Create(2, "Bicycle").Value,
            Location.Create(1, 1).Value
        ).Value;
        var newLocation = Location.Create(2, 2).Value;

        // Act
        var result = courier.MoveTo(newLocation);

        // Assert
        result.IsSuccess.Should().BeTrue();
        courier.Location.Should().Be(newLocation);
    }

    [Fact]
    public void MoveTo_WhenBusy_ShouldReturnFailure()
    {
        // Arrange
        var courier = Courier.Create(
            Guid.NewGuid(),
            "John Doe",
            Transport.Create(2, "Bicycle").Value,
            Location.Create(1, 1).Value
        ).Value;
        courier.SetStatusToBusy();
        var newLocation = Location.Create(2, 2).Value;

        // Act
        var result = courier.MoveTo(newLocation);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeneralErrors.CantMoveBusyCourier());
    }

    [Fact]
    public void CalculateTimeToLocation_WhenFree_ShouldReturnTime()
    {
        // Arrange
        var courier = Courier.Create(
            Guid.NewGuid(),
            "John Doe",
            Transport.Create(2, "Bicycle").Value,
            Location.Create(1, 1).Value
        ).Value;
        var targetLocation = Location.Create(3, 3).Value;

        // Act
        var result = courier.CalculateTimeToLocation(targetLocation);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(2.0); // Distance is 4 (3-1 + 3-1), speed is 2, so time is 2
    }

    [Fact]
    public void CalculateTimeToLocation_WhenBusy_ShouldReturnFailure()
    {
        // Arrange
        var courier = Courier.Create(
            Guid.NewGuid(),
            "John Doe",
            Transport.Create(2, "Bicycle").Value,
            Location.Create(1, 1).Value
        ).Value;
        courier.SetStatusToBusy();
        var targetLocation = Location.Create(3, 3).Value;

        // Act
        var result = courier.CalculateTimeToLocation(targetLocation);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeneralErrors.CantMoveBusyCourier());
    }
} 