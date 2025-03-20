using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.IntegrationTests.Infrastructure.Repositories;

public class OrderRepositoryTests : BaseRepositoryTests
{
    private OrderRepository _repository;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _repository = new OrderRepository(DbContext);
    }

    [Fact]
    public async Task AddAsync_ShouldAddOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var location = Location.Create(1, 1).Value;
        var orderResult = Order.Create(orderId, location);
        orderResult.IsSuccess.Should().BeTrue();
        var order = orderResult.Value;

        // Act
        await _repository.AddAsync(order);
        await DbContext.SaveChangesAsync();

        // Assert
        var savedOrder = await _repository.GetAsync(order.Id);
        savedOrder.HasValue.Should().BeTrue();
        savedOrder.Value.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task GetFirstInCreatedStatus_ShouldReturnCreatedOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var location = Location.Create(1, 1).Value;
        var orderResult = Order.Create(orderId, location);
        orderResult.IsSuccess.Should().BeTrue();
        var order = orderResult.Value;
        await _repository.AddAsync(order);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetFirstInCreatedStatus();

        // Assert
        result.HasValue.Should().BeTrue();
        result.Value.Id.Should().Be(order.Id);
        result.Value.Status.Name.Should().Be(OrderStatus.Created.Name);
    }

    [Fact]
    public async Task GetAllAssigned_ShouldReturnAssignedOrders()
    {
        // Arrange
        var order1Id = Guid.NewGuid();
        var order2Id = Guid.NewGuid();
        var location = Location.Create(1, 1).Value;
        
        var order1Result = Order.Create(order1Id, location);
        var order2Result = Order.Create(order2Id, location);
        order1Result.IsSuccess.Should().BeTrue();
        order2Result.IsSuccess.Should().BeTrue();
        
        var order1 = order1Result.Value;
        var order2 = order2Result.Value;
        
        await _repository.AddAsync(order1);
        await _repository.AddAsync(order2);
        await DbContext.SaveChangesAsync();

        // Create a courier to assign orders to
        var courierId = Guid.NewGuid();
        var courierName = "Test Courier";
        var transportResult = Transport.Create(1, "Bike");
        transportResult.IsSuccess.Should().BeTrue();
        var transport = transportResult.Value;
        var courierResult = Courier.Create(courierId, courierName, transport, location);
        courierResult.IsSuccess.Should().BeTrue();
        var courier = courierResult.Value;

        order1.Assign(courier);
        order2.Assign(courier);
        await DbContext.SaveChangesAsync();

        // Act
        var assignedOrders = _repository.GetAllAssigned().ToList();

        // Assert
        assignedOrders.Should().HaveCount(2);
        assignedOrders.Should().AllSatisfy(o => o.Status.Name.Should().Be(OrderStatus.Assigned.Name));
    }

    [Fact]
    public async Task Update_ShouldUpdateOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var location = Location.Create(1, 1).Value;
        var orderResult = Order.Create(orderId, location);
        orderResult.IsSuccess.Should().BeTrue();
        var order = orderResult.Value;
        await _repository.AddAsync(order);
        await DbContext.SaveChangesAsync();

        // Create a courier to assign the order to
        var courierId = Guid.NewGuid();
        var courierName = "Test Courier";
        var transportResult = Transport.Create(1, "Bike");
        transportResult.IsSuccess.Should().BeTrue();
        var transport = transportResult.Value;
        var courierResult = Courier.Create(courierId, courierName, transport, location);
        courierResult.IsSuccess.Should().BeTrue();
        var courier = courierResult.Value;

        // Act
        order.Assign(courier);
        _repository.Update(order);
        await DbContext.SaveChangesAsync();

        // Assert
        var updatedOrder = await _repository.GetAsync(order.Id);
        updatedOrder.HasValue.Should().BeTrue();
        updatedOrder.Value.Status.Name.Should().Be(OrderStatus.Assigned.Name);
    }
} 