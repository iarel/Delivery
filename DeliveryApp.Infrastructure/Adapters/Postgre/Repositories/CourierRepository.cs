using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgre.Repositories;

public class CourierRepository : ICourierRepository
{
    private readonly ApplicationDbContext _dbContext;
    public CourierRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    public async Task AddAsync(Courier courier)
    {
        await _dbContext.Couriers.AddAsync(courier);
    }
    public void Update(Courier courier)
    {
        _dbContext.Couriers.Update(courier);
    }
    public async Task<Maybe<Courier>> GetAsync(Guid courierId)
    {
        var courier = await _dbContext
            .Couriers
            .Include(x => x.Transport)
            .FirstOrDefaultAsync(o => o.Id == courierId);
        return courier;
    }
    public async Task<IEnumerable<Courier>> GetAllFree()
    {
        return await _dbContext
            .Couriers
            .Where(o => o.Status.Name == CourierStatus.Free.Name)
            .Include(x => x.Transport)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Courier>> GetAllAsync()
    {
        return await _dbContext
            .Couriers
            .Include(x => x.Transport)
            .ToListAsync();
    }
}