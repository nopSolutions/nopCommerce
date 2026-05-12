using Nop.Data;
using Nop.Plugin.Misc.OmnichannelCore.Domains;

namespace Nop.Plugin.Misc.OmnichannelCore.Services;

/// <summary>
/// Represents omnichannel core read service
/// </summary>
public class OmnichannelCoreService
{
    #region Fields

    private readonly IRepository<OmniInboxMessage> _inboxMessageRepository;
    private readonly IRepository<OmniOrderFulfillment> _orderFulfillmentRepository;
    private readonly IRepository<OmniOutboxMessage> _outboxMessageRepository;
    private readonly IRepository<OmniStockSyncState> _stockSyncStateRepository;

    #endregion

    #region Ctor

    public OmnichannelCoreService(IRepository<OmniInboxMessage> inboxMessageRepository,
        IRepository<OmniOrderFulfillment> orderFulfillmentRepository,
        IRepository<OmniOutboxMessage> outboxMessageRepository,
        IRepository<OmniStockSyncState> stockSyncStateRepository)
    {
        _inboxMessageRepository = inboxMessageRepository;
        _orderFulfillmentRepository = orderFulfillmentRepository;
        _outboxMessageRepository = outboxMessageRepository;
        _stockSyncStateRepository = stockSyncStateRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the outbox message count
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<int> GetOutboxMessageCountAsync()
    {
        return await _outboxMessageRepository.Table.CountAsync();
    }

    /// <summary>
    /// Gets the inbox message count
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<int> GetInboxMessageCountAsync()
    {
        return await _inboxMessageRepository.Table.CountAsync();
    }

    /// <summary>
    /// Gets the fulfillment record count
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<int> GetFulfillmentRecordCountAsync()
    {
        return await _orderFulfillmentRepository.Table.CountAsync();
    }

    /// <summary>
    /// Gets the stock projection record count
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<int> GetStockProjectionRecordCountAsync()
    {
        return await _stockSyncStateRepository.Table.CountAsync();
    }

    #endregion
}
