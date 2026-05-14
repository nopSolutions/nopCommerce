using Nop.Plugin.Inventory.AllocationGate.Domain;
using Nop.Services.Catalog;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Inventory.AllocationGate.Services;

public class ReleaseExpiredReservationsTask : IScheduleTask
{
    private readonly IProductReservationRepository _reservationRepository;
    private readonly IProductService _productService;

    public ReleaseExpiredReservationsTask(IProductReservationRepository reservationRepository, IProductService productService)
    {
        _reservationRepository = reservationRepository;
        _productService = productService;
    }

    public async Task ExecuteAsync()
    {
        var expired = await _reservationRepository.GetExpiredAsync();
        foreach (var reservation in expired)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(reservation.ProductId);
                if (product is not null)
                {
                    product.StockQuantity += reservation.Quantity;
                    await _productService.UpdateProductAsync(product);
                }

                reservation.Status = (int)ReservationStatus.Expired;
                reservation.ReservedUntilUtc = null;
                await _reservationRepository.UpdateAsync(reservation);
            }
            catch
            {
                // never let one failure block the rest
            }
        }
    }
}
