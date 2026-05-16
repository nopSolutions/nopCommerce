using Nop.Plugin.Inventory.AllocationGate.Domain;

namespace Nop.Plugin.Inventory.AllocationGate.Services;

public interface IProductReservationRepository
{
    Task InsertAsync(ProductReservation reservation);
    Task<ProductReservation> GetByKeyAsync(string reservationKey);
    Task UpdateAsync(ProductReservation reservation);
    Task<IList<ProductReservation>> GetExpiredAsync(int batchSize);
}
