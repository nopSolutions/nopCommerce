using System.Reflection;
using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Configuration;

namespace Nop.Plugin.Inventory.AllocationGate.Services;

public class AllocationGateProductServiceDecorator : DispatchProxy
{
    private IProductService _inner = null!;
    private IAllocationGate _gate = null!;
    private ILogger _logger = null!;
    private ISettingService _settingService = null!;
    private IStoreContext _storeContext = null!;

    public static IProductService Create(
        IProductService inner,
        IAllocationGate gate,
        ILogger logger,
        ISettingService settingService,
        IStoreContext storeContext)
    {
        var proxy = Create<IProductService, AllocationGateProductServiceDecorator>();
        var handler = (AllocationGateProductServiceDecorator)(object)proxy;
        handler._inner = inner;
        handler._gate = gate;
        handler._logger = logger;
        handler._settingService = settingService;
        handler._storeContext = storeContext;
        return proxy;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        // Intercept only decrements (negative quantityToChange = stock being consumed at checkout)
        if (targetMethod?.Name == nameof(IProductService.AdjustInventoryAsync) &&
            args?.Length >= 2 &&
            args[0] is Product product &&
            args[1] is int qty &&
            qty < 0)
        {
            return HandleDecrementAsync(product, qty);
        }

        return targetMethod?.Invoke(_inner, args);
    }

    private async Task HandleDecrementAsync(Product product, int quantityToChange)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var settings = await _settingService.LoadSettingAsync<AllocationSettings>(store.Id);

        var reservationKey = $"web-{product.Id}-{Guid.NewGuid():N}";

        var result = await _gate.ReserveAsync(
            productId: product.Id,
            warehouseId: product.WarehouseId,
            quantity: -quantityToChange,
            channelKey: "web",
            reservationKey: reservationKey,
            ttlSeconds: settings.WebReservationTtlSeconds);

        if (!result.Success)
        {
            _logger.LogWarning("[AllocationGate] Web checkout blocked for ProductId={ProductId}: {Reason}",
                product.Id, result.Message);
            throw new NopException("The quantity of the selected product is not available.");
        }

        await _gate.ConfirmAsync(reservationKey);

        _logger.LogInformation("[AllocationGate] Web checkout reserved and confirmed for ProductId={ProductId} Qty={Qty}",
            product.Id, -quantityToChange);
    }
}
