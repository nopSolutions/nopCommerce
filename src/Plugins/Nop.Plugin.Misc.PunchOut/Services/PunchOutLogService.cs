using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.PunchOut.Domain;

namespace Nop.Plugin.Misc.PunchOut.Services;

/// <summary>
/// Represents the PunchOut log service implementation
/// </summary>
public class PunchOutLogService
{
    #region Fields

    protected readonly IRepository<PunchOutLog> _punchOutLogRepository;

    #endregion

    #region Ctor

    public PunchOutLogService(IRepository<PunchOutLog> punchOutLogRepository)
    {
        _punchOutLogRepository = punchOutLogRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get PunchOut log
    /// </summary>
    /// <param name="createdFromUtc">Log item creation from; pass null to load all records</param>
    /// <param name="createdToUtc">Log item creation to; pass null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of punch out log items
    /// </returns>
    public async Task<IPagedList<PunchOutLog>> GetPunchOutLogAsync(DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        //get all logs
        var query = _punchOutLogRepository.Table;

        //filter by dates
        if (createdFromUtc.HasValue)
            query = query.Where(logItem => logItem.CreatedOnUtc >= createdFromUtc.Value);
        if (createdToUtc.HasValue)
            query = query.Where(logItem => logItem.CreatedOnUtc <= createdToUtc.Value);

        //order log records
        query = query.OrderByDescending(logItem => logItem.CreatedOnUtc).ThenByDescending(logItem => logItem.Id);

        //return paged log
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Insert the log item
    /// </summary>
    /// <param name="logItem">Log item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task LogAsync(PunchOutLog punchOutLog)
    {
        ArgumentNullException.ThrowIfNull(punchOutLog);

        punchOutLog.CreatedOnUtc = DateTime.UtcNow;
        await _punchOutLogRepository.InsertAsync(punchOutLog, false);
    }

    /// <summary>
    /// Get a log item by the identifier
    /// </summary>
    /// <param name="logItemId">Log item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the log item
    /// </returns>
    public async Task<PunchOutLog> GetPunchOutLogByIdAsync(int logItemId)
    {
        return await _punchOutLogRepository.GetByIdAsync(logItemId);
    }

    /// <summary>
    /// Delete the log item
    /// </summary>
    /// <param name="logItem">Log item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteLogItemAsync(PunchOutLog logItem)
    {
        await _punchOutLogRepository.DeleteAsync(logItem, false);
    }

    /// <summary>
    /// Clear the log
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ClearLogAsync()
    {
        await _punchOutLogRepository.TruncateAsync();
    }

    #endregion
}
