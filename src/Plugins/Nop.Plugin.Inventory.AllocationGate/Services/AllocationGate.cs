using System.Collections.Concurrent;
using Nop.Plugin.Inventory.AllocationGate.Domain;
using Nop.Services.Catalog;

namespace Nop.Plugin.Inventory.AllocationGate.Services;

public class AllocationGateService : IAllocationGate
{
    private static readonly ConcurrentDictionary<int, SemaphoreSlim> _productLocks = new();

    private readonly IProductService _productService;
    private readonly IProductReservationRepository _reservationRepository;

    public AllocationGateService(IProductService productService, IProductReservationRepository reservationRepository)
    {
        _productService = productService;
        _reservationRepository = reservationRepository;
    }

    public async Task<AllocationResult> ReserveAsync(int productId, int quantity, string channelKey, string reservationKey, int ttlSeconds = 300)
    {
        var sem = _productLocks.GetOrAdd(productId, _ => new SemaphoreSlim(1, 1));
        await sem.WaitAsync();
        try
        {
            var existing = await _reservationRepository.GetByKeyAsync(reservationKey);
            if (existing is not null)
                return new AllocationResult(true, "already-reserved");

            var product = await _productService.GetProductByIdAsync(productId);
            if (product is null || product.StockQuantity < quantity)
                return new AllocationResult(false, "insufficient-stock");

            product.StockQuantity -= quantity;
            await _productService.UpdateProductAsync(product);

            await _reservationRepository.InsertAsync(new ProductReservation
            {
                ReservationKey = reservationKey,
                ChannelKey = channelKey,
                ProductId = productId,
                Quantity = quantity,
                Status = (int)ReservationStatus.Active,
                ReservedUntilUtc = DateTime.UtcNow.AddSeconds(ttlSeconds),
                CreatedOnUtc = DateTime.UtcNow
            });

            return new AllocationResult(true);
        }
        finally
        {
            sem.Release();
        }
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

        var product = await _productService.GetProductByIdAsync(reservation.ProductId);
        if (product is not null)
        {
            product.StockQuantity += reservation.Quantity;
            await _productService.UpdateProductAsync(product);
        }

        reservation.Status = (int)ReservationStatus.Released;
        reservation.ReservedUntilUtc = null;
        await _reservationRepository.UpdateAsync(reservation);
        return true;
    }
}
