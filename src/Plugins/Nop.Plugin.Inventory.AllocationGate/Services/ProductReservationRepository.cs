using Nop.Data;
using Nop.Plugin.Inventory.AllocationGate.Domain;

namespace Nop.Plugin.Inventory.AllocationGate.Services;

public class ProductReservationRepository : IProductReservationRepository
{
    private readonly IRepository<ProductReservation> _repository;

    public ProductReservationRepository(IRepository<ProductReservation> repository)
    {
        _repository = repository;
    }

    public async Task InsertAsync(ProductReservation reservation)
        => await _repository.InsertAsync(reservation, publishEvent: false);

    public async Task<ProductReservation> GetByKeyAsync(string reservationKey)
        => await _repository.Table
            .Where(r => r.ReservationKey == reservationKey)
            .FirstOrDefaultAsync();

    public async Task UpdateAsync(ProductReservation reservation)
        => await _repository.UpdateAsync(reservation, publishEvent: false);

    public async Task<IList<ProductReservation>> GetExpiredAsync()
        => await _repository.Table
            .Where(r => r.Status == (int)ReservationStatus.Active && r.ReservedUntilUtc != null && r.ReservedUntilUtc < DateTime.UtcNow)
            .ToListAsync();
}
