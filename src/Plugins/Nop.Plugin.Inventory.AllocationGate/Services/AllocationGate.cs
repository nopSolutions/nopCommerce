using System.Transactions;
using LinqToDB.Data;
using Nop.Data;
using Nop.Plugin.Inventory.AllocationGate.Domain;

namespace Nop.Plugin.Inventory.AllocationGate.Services;

public class AllocationGateService : IAllocationGate
{
    private readonly INopDataProvider _dataProvider;
    private readonly IProductReservationRepository _reservationRepository;

    public AllocationGateService(INopDataProvider dataProvider, IProductReservationRepository reservationRepository)
    {
        _dataProvider = dataProvider;
        _reservationRepository = reservationRepository;
    }

    public async Task<AllocationResult> ReserveAsync(int productId, int warehouseId, int quantity, string channelKey, string reservationKey, int ttlSeconds = 300)
    {
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        var existing = await _reservationRepository.GetByKeyAsync(reservationKey);
        if (existing is not null)
        {
            scope.Complete();
            return new AllocationResult(true, "already-reserved");
        }

        // Atomic decrement: SQL Server serialises concurrent UPDATEs on the same row —
        // the WHERE StockQuantity >= @qty acts as the availability check inside the lock.
        var rows = await _dataProvider.ExecuteNonQueryAsync(
            "UPDATE Product SET StockQuantity = StockQuantity - @qty WHERE Id = @id AND StockQuantity >= @qty",
            new DataParameter("@qty", quantity),
            new DataParameter("@id", productId));

        if (rows == 0)
            return new AllocationResult(false, "insufficient-stock");

        await _reservationRepository.InsertAsync(new ProductReservation
        {
            ReservationKey = reservationKey,
            ChannelKey = channelKey,
            ProductId = productId,
            WarehouseId = warehouseId,
            Quantity = quantity,
            Status = (int)ReservationStatus.Active,
            ReservedUntilUtc = DateTime.UtcNow.AddSeconds(ttlSeconds),
            CreatedOnUtc = DateTime.UtcNow
        });

        scope.Complete();
        return new AllocationResult(true, "reserved");
    }

    public async Task<bool> ConfirmAsync(string reservationKey)
    {
        var reservation = await _reservationRepository.GetByKeyAsync(reservationKey);
        if (reservation is null || reservation.Status != (int)ReservationStatus.Active)
            return false;

        reservation.Status = (int)ReservationStatus.Committed;
        reservation.ReservedUntilUtc = null;
        await _reservationRepository.UpdateAsync(reservation);
        return true;
    }

    public async Task<bool> ReleaseAsync(string reservationKey)
    {
        var reservation = await _reservationRepository.GetByKeyAsync(reservationKey);
        if (reservation is null || reservation.Status != (int)ReservationStatus.Active)
            return false;

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _dataProvider.ExecuteNonQueryAsync(
            "UPDATE Product SET StockQuantity = StockQuantity + @qty WHERE Id = @id",
            new DataParameter("@qty", reservation.Quantity),
            new DataParameter("@id", reservation.ProductId));

        reservation.Status = (int)ReservationStatus.Released;
        reservation.ReservedUntilUtc = null;
        await _reservationRepository.UpdateAsync(reservation);

        scope.Complete();
        return true;
    }
}
