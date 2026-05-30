using Nop.Data;
using Nop.Plugin.Misc.OmnichannelCore.Domains;
using Nop.Plugin.Misc.OmnichannelCore.Models.Callbacks;

namespace Nop.Plugin.Misc.OmnichannelCore.Services;

/// <summary>
/// Represents cross-channel stock projection service
/// </summary>
public class OmniStockSyncService
{
    #region Fields

    private readonly IRepository<OmniStockSyncState> _stockSyncStateRepository;

    #endregion

    #region Ctor

    public OmniStockSyncService(IRepository<OmniStockSyncState> stockSyncStateRepository)
    {
        _stockSyncStateRepository = stockSyncStateRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Applies a POS stock changed event to the projection
    /// </summary>
    /// <param name="request">POS stock changed event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<StockSyncResult> ApplyPosStockChangedAsync(PosStockChangedRequest request)
    {
        var stockSyncState = await _stockSyncStateRepository.Table
            .FirstOrDefaultAsync(state => state.ProductId == request.ProductId &&
                                          state.WarehouseId == request.WarehouseId);

        if (stockSyncState == null)
            return await InsertStockSyncStateAsync(request);

        if (request.SourceVersion <= stockSyncState.SourceVersion)
            return await MarkStaleAsync(stockSyncState, request);

        stockSyncState.Sku = request.Sku;
        stockSyncState.QuantityOnHand = request.QuantityOnHand;
        stockSyncState.SourceVersion = request.SourceVersion;
        stockSyncState.LastMessageId = request.MessageId;
        stockSyncState.Source = request.Source;
        stockSyncState.Status = OmniStockSyncStatus.Current;
        stockSyncState.LastSeenOnUtc = DateTime.UtcNow;
        stockSyncState.UpdatedOnUtc = stockSyncState.LastSeenOnUtc;

        await _stockSyncStateRepository.UpdateAsync(stockSyncState);

        return new StockSyncResult
        {
            Result = "applied",
            Applied = true,
            Stale = false,
            StockSyncState = stockSyncState,
            Detail = "stock projection updated"
        };
    }

    #endregion

    #region Utilities

    private async Task<StockSyncResult> InsertStockSyncStateAsync(PosStockChangedRequest request)
    {
        var now = DateTime.UtcNow;
        var stockSyncState = new OmniStockSyncState
        {
            ProductId = request.ProductId,
            Sku = request.Sku,
            WarehouseId = request.WarehouseId,
            QuantityOnHand = request.QuantityOnHand,
            SourceVersion = request.SourceVersion,
            LastMessageId = request.MessageId,
            Source = request.Source,
            Status = OmniStockSyncStatus.Current,
            LastSeenOnUtc = now,
            UpdatedOnUtc = now
        };

        await _stockSyncStateRepository.InsertAsync(stockSyncState);

        return new StockSyncResult
        {
            Result = "applied",
            Applied = true,
            Stale = false,
            StockSyncState = stockSyncState,
            Detail = "stock projection inserted"
        };
    }

    private async Task<StockSyncResult> MarkStaleAsync(OmniStockSyncState stockSyncState, PosStockChangedRequest request)
    {
        stockSyncState.LastMessageId = request.MessageId;
        stockSyncState.Source = request.Source;
        stockSyncState.Status = OmniStockSyncStatus.StaleIgnored;
        stockSyncState.LastSeenOnUtc = DateTime.UtcNow;
        stockSyncState.UpdatedOnUtc = stockSyncState.LastSeenOnUtc;

        await _stockSyncStateRepository.UpdateAsync(stockSyncState);

        return new StockSyncResult
        {
            Result = "stale_ignored",
            Applied = false,
            Stale = true,
            StockSyncState = stockSyncState,
            Detail = $"sourceVersion {request.SourceVersion} is not newer than stored version {stockSyncState.SourceVersion}"
        };
    }

    #endregion
}
