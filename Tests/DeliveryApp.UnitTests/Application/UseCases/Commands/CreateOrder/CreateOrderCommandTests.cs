using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.UseCases.Commands;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Ports;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using NSubstitute;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Application.UseCases.Commands.CreateOrder;

public class CreateOrderCommandTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    private readonly CreateOrderHandler _handler;

    public CreateOrderCommandTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _orderRepository = Substitute.For<IOrderRepository>();
        _handler = new CreateOrderHandler(_unitOfWork, _orderRepository);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateOrder()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var command = new CreateOrderCommand(basketId, "Test Street");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _orderRepository.Received(1).AddAsync(Arg.Any<Order>());
        await _unitOfWork.Received(1).SaveChangesAsync(cancellationToken);
    }

    [Fact]
    public void Constructor_WithInvalidBasketId_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => new CreateOrderCommand(Guid.Empty, "Test Street");
        act.Should().Throw<ArgumentException>()
            .WithMessage("BasketId cannot be empty (Parameter 'basketId')");
    }

    [Fact]
    public void Constructor_WithEmptyStreet_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => new CreateOrderCommand(Guid.NewGuid(), string.Empty);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Street cannot be empty (Parameter 'street')");
    }
} 