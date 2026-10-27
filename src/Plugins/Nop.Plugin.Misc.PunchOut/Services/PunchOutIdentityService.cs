using System.Text;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.PunchOut.Domain;

namespace Nop.Plugin.Misc.PunchOut.Services;

/// <summary>
/// Represents the PunchOut sender identity service implementation
/// </summary>
public class PunchOutIdentityService
{
    #region Fields

    protected readonly IRepository<PunchOutIdentity> _punchOutIdentityRepository;

    #endregion

    #region Ctor

    public PunchOutIdentityService(IRepository<PunchOutIdentity> punchOutIdentityRepository)
    {
        _punchOutIdentityRepository = punchOutIdentityRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get PunchOut identities
    /// </summary>
    /// <param name="identity">Identity</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of PunchOut identities
    /// </returns>
    public async Task<IPagedList<PunchOutIdentity>> GetPunchOutIdentitiesAsync(string identity,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        //get all items
        var query = _punchOutIdentityRepository.Table;

        //filter by Identity
        if (!string.IsNullOrEmpty(identity))
            query = query.Where(item => item.Identity.Contains(identity));

        //order item records
        query = query.OrderByDescending(item => item.Id);

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Get PunchOut identity
    /// </summary>
    /// <param name="identity">Identity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the PunchOut identity
    /// </returns>
    public async Task<PunchOutIdentity> GetPunchOutIdentityAsync(string identity)
    {
        if (string.IsNullOrEmpty(identity))
            return null;

        return await _punchOutIdentityRepository.Table
            .FirstOrDefaultAsync(item => item.Identity == identity);
    }

    /// <summary>
    /// Gets a PunchOut identity by identifier
    /// </summary>
    /// <param name="identityId">PunchOut identity identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the punchOut identity
    /// </returns>
    public async Task<PunchOutIdentity> GetPunchOutIdentityByIdAsync(int identityId)
    {
        return await _punchOutIdentityRepository.GetByIdAsync(identityId);
    }

    /// <summary>
    /// Add PunchOut identity
    /// </summary>
    /// <param name="identity">PunchOut identity</param>
    /// <param name="sharedSecret">Shared secret</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task AddPunchOutIdentityAsync(string identity, string sharedSecret)
    {
        if (string.IsNullOrEmpty(identity) || string.IsNullOrEmpty(sharedSecret))
            return;

        var record = new PunchOutIdentity
        {
            Identity = identity,
            SharedSecretHash = HashHelper.CreateHash(Encoding.UTF8.GetBytes(sharedSecret), "SHA256")
        };

        await _punchOutIdentityRepository.InsertAsync(record, false);
    }

    /// <summary>
    /// Update the PunchOut identity
    /// </summary>
    /// <param name="identity">PunchOut identity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdatePunchOutIdentityAsync(PunchOutIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(identity);

        await _punchOutIdentityRepository.UpdateAsync(identity, false);
    }

    /// <summary>
    /// Delete PunchOut identity
    /// </summary>
    /// <param name="id">PunchOut identity identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeletePunchOutIdentityAsync(int id)
    {
        await _punchOutIdentityRepository.DeleteAsync(item => item.Id == id);
    }

    #endregion
}
