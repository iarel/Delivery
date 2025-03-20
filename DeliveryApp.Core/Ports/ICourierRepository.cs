using DeliveryApp.Core.Domain.Model.CourierAggregate;
using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Ports;

public interface ICourierRepository
{
    Task AddAsync(Courier courier);
    void Update(Courier courier);
    Task<Maybe<Courier>> GetAsync(Guid courierId);
    IEnumerable<Courier> GetAllFree();
}