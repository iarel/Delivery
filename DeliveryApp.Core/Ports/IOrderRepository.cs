using DeliveryApp.Core.Domain.Model.OrderAggregate;
using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Ports;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    void Update(Order order);
    Task<Maybe<Order>> GetAsync(Guid orderId);
    Task<Maybe<Order>> GetFirstInCreatedStatus();
    IEnumerable<Order> GetAllAssigned();
}