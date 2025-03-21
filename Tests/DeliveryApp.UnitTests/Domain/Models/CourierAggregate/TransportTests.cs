using System;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.CourierAggregate;

public class TransportTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateTransport()
    {
        // Arrange
        var speed = 2;
        var name = "Bicycle";

        // Act
        var result = Transport.Create(speed, name);

        // Assert
        Assert.True(result.IsSuccess);
        var transport = result.Value;
        Assert.Equal(speed, transport.Speed);
        Assert.Equal(name, transport.Name);
        Assert.NotEqual(Guid.Empty, transport.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(4)]
    public void Create_WithInvalidSpeed_ShouldReturnFailure(int invalidSpeed)
    {
        // Arrange
        var name = "Bicycle";

        // Act
        var result = Transport.Create(invalidSpeed, name);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Speed must be between 1 and 3", result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidName_ShouldReturnFailure(string invalidName)
    {
        // Arrange
        var speed = 2;

        // Act
        var result = Transport.Create(speed, invalidName);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Name must be provided", result.Error);
    }

    [Fact]
    public void Create_WithValidParameters_ShouldCreateUniqueIds()
    {
        // Arrange
        var speed = 2;
        var name = "Bicycle";

        // Act
        var result1 = Transport.Create(speed, name);
        var result2 = Transport.Create(speed, name);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.NotEqual(result1.Value.Id, result2.Value.Id);
    }
} 