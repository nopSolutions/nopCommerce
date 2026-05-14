namespace Nop.Plugin.Inventory.AllocationGate.Services;

public interface IAllocationGate
{
    Task<AllocationResult> ReserveAsync(int productId, int quantity, string channelKey, string reservationKey, int ttlSeconds = 300);
    Task<bool> ConfirmAsync(string reservationKey);
    Task<bool> ReleaseAsync(string reservationKey);
}

public record AllocationResult(bool Success, string Message = null);
