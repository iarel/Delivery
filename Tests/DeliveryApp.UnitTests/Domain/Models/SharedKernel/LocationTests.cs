using DeliveryApp.Core.Domain.SharedKernel;
using Shouldly;
using Xunit;
using System.Linq;

namespace DeliveryApp.UnitTests.Domain.Models.SharedKernel;

public class LocationTests
{
    [Fact]
    public void Create_WithValidCoordinates_ShouldSucceed()
    {
        // Arrange
        var x = 5;
        var y = 5;

        // Act
        var result = Location.Create(x, y);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.X.ShouldBe(x);
        result.Value.Y.ShouldBe(y);
    }

    [Theory]
    [InlineData(0, 5, "X must be between 1 and 10")]
    [InlineData(11, 5, "X must be between 1 and 10")]
    [InlineData(5, 0, "Y must be between 1 and 10")]
    [InlineData(5, 11, "Y must be between 1 and 10")]
    public void Create_WithInvalidCoordinates_ShouldFail(int x, int y, string expectedError)
    {
        // Act
        var result = Location.Create(x, y);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(expectedError);
    }

    [Fact]
    public void MaxLocation_ShouldHaveCorrectValues()
    {
        // Assert
        Location.MaxLocation.X.ShouldBe(10);
        Location.MaxLocation.Y.ShouldBe(10);
    }

    [Fact]
    public void MinLocation_ShouldHaveCorrectValues()
    {
        // Assert
        Location.MinLocation.X.ShouldBe(1);
        Location.MinLocation.Y.ShouldBe(1);
    }

    [Fact]
    public void TwoLocationsWithSameCoordinates_ShouldBeEqual()
    {
        // Arrange
        var location1 = Location.Create(5, 5).Value;
        var location2 = Location.Create(5, 5).Value;

        // Assert
        location1.ShouldBe(location2);
    }

    [Fact]
    public void TwoLocationsWithDifferentCoordinates_ShouldNotBeEqual()
    {
        // Arrange
        var location1 = Location.Create(5, 5).Value;
        var location2 = Location.Create(5, 6).Value;

        // Assert
        location1.ShouldNotBe(location2);
    }

    [Theory]
    [InlineData(1, 1, 1, 1, 0)]  // Same location
    [InlineData(1, 1, 1, 2, 1)]  // One step vertical
    [InlineData(1, 1, 2, 1, 1)]  // One step horizontal
    [InlineData(1, 1, 2, 2, 2)]  // One step horizontal + one step vertical
    [InlineData(1, 1, 10, 10, 18)]  // Maximum distance (9 steps horizontal + 9 steps vertical)
    public void CalculateDistanceTo_ShouldReturnCorrectManhattanDistance(
        int x1, int y1, int x2, int y2, int expectedDistance)
    {
        // Arrange
        var location1 = Location.Create(x1, y1).Value;
        var location2 = Location.Create(x2, y2).Value;

        // Act
        var distance = location1.CalculateDistanceTo(location2);

        // Assert
        distance.ShouldBe(expectedDistance);
    }

    [Fact]
    public void RandomLocation_ShouldGenerateValidCoordinates()
    {
        // Act
        var location = Location.RandomLocation();

        // Assert
        location.X.ShouldBeGreaterThanOrEqualTo(1);
        location.X.ShouldBeLessThanOrEqualTo(10);
        location.Y.ShouldBeGreaterThanOrEqualTo(1);
        location.Y.ShouldBeLessThanOrEqualTo(10);
    }
} 