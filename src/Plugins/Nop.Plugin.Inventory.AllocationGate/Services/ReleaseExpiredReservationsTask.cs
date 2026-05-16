using Nop.Core;
using Nop.Data;
using Nop.Plugin.Inventory.AllocationGate.Domain;
using Nop.Services.Configuration;
using Nop.Services.ScheduleTasks;
using System.Transactions;
using LinqToDB.Data;

namespace Nop.Plugin.Inventory.AllocationGate.Services;

public class ReleaseExpiredReservationsTask : IScheduleTask
{
    private readonly IProductReservationRepository _reservationRepository;
    private readonly INopDataProvider _dataProvider;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;

    public ReleaseExpiredReservationsTask(
        IProductReservationRepository reservationRepository,
        INopDataProvider dataProvider,
        ISettingService settingService,
        IStoreContext storeContext)
    {
        _reservationRepository = reservationRepository;
        _dataProvider = dataProvider;
        _settingService = settingService;
        _storeContext = storeContext;
    }

    public async Task ExecuteAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var settings = await _settingService.LoadSettingAsync<AllocationSettings>(store.Id);

        var expired = await _reservationRepository.GetExpiredAsync(settings.ReleaseTaskBatchSize);

        foreach (var reservation in expired)
        {
            try
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                await _dataProvider.ExecuteNonQueryAsync(
                    "UPDATE Product SET StockQuantity = StockQuantity + @qty WHERE Id = @id",
                    new DataParameter("@qty", reservation.Quantity),
                    new DataParameter("@id", reservation.ProductId));

                reservation.Status = (int)ReservationStatus.Expired;
                reservation.ReservedUntilUtc = null;
                await _reservationRepository.UpdateAsync(reservation);

                scope.Complete();
            }
            catch
            {
                // never let one failure block the rest
            }
        }
    }
}
