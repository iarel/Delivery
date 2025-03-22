using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgre.Repositories;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.IntegrationTests.Infrastructure.Repositories;

public class CourierRepositoryTests : BaseRepositoryTests
{
    private CourierRepository _repository;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _repository = new CourierRepository(DbContext);
    }

    [Fact]
    public async Task AddAsync_ShouldAddCourier()
    {
        // Arrange
        var courierId = Guid.NewGuid();
        var courierName = "Test Courier";
        var location = Location.Create(1, 1).Value;
        var transportResult = Transport.Create(1, "Bike");
        transportResult.IsSuccess.Should().BeTrue();
        var transport = transportResult.Value;
        
        var courierResult = Courier.Create(courierId, courierName, transport, location);
        courierResult.IsSuccess.Should().BeTrue();
        var courier = courierResult.Value;

        // Act
        await _repository.AddAsync(courier);
        await DbContext.SaveChangesAsync();

        // Assert
        var savedCourier = await _repository.GetAsync(courier.Id);
        savedCourier.HasValue.Should().BeTrue();
        savedCourier.Value.Id.Should().Be(courier.Id);
    }

    [Fact]
    public async Task GetAllFree_ShouldReturnFreeCouriers()
    {
        // Arrange
        var location = Location.Create(1, 1).Value;
        var transportResult = Transport.Create(1, "Bike");
        transportResult.IsSuccess.Should().BeTrue();
        var transport = transportResult.Value;

        var courier1Id = Guid.NewGuid();
        var courier2Id = Guid.NewGuid();
        var courier1Result = Courier.Create(courier1Id, "Courier 1", transport, location);
        var courier2Result = Courier.Create(courier2Id, "Courier 2", transport, location);
        courier1Result.IsSuccess.Should().BeTrue();
        courier2Result.IsSuccess.Should().BeTrue();
        
        var courier1 = courier1Result.Value;
        var courier2 = courier2Result.Value;
        
        await _repository.AddAsync(courier1);
        await _repository.AddAsync(courier2);
        await DbContext.SaveChangesAsync();

        // Act
        var freeCouriers = (await _repository.GetAllFree()).ToList();

        // Assert
        freeCouriers.Should().HaveCount(2);
        freeCouriers.Should().AllSatisfy(c => c.Status.Name.Should().Be(CourierStatus.Free.Name));
    }

    [Fact]
    public async Task Update_ShouldUpdateCourier()
    {
        // Arrange
        var courierId = Guid.NewGuid();
        var courierName = "Test Courier";
        var location = Location.Create(1, 1).Value;
        var transportResult = Transport.Create(1, "Bike");
        transportResult.IsSuccess.Should().BeTrue();
        var transport = transportResult.Value;
        
        var courierResult = Courier.Create(courierId, courierName, transport, location);
        courierResult.IsSuccess.Should().BeTrue();
        var courier = courierResult.Value;
        
        await _repository.AddAsync(courier);
        await DbContext.SaveChangesAsync();

        // Act
        var setStatusResult = courier.SetStatusToBusy();
        setStatusResult.IsSuccess.Should().BeTrue();
        _repository.Update(courier);
        await DbContext.SaveChangesAsync();

        // Assert
        var updatedCourier = await _repository.GetAsync(courier.Id);
        updatedCourier.HasValue.Should().BeTrue();
        updatedCourier.Value.Status.Name.Should().Be(CourierStatus.Busy.Name);
    }
} 