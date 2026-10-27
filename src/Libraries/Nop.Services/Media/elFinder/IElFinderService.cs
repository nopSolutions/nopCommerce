using elFinder.NetCore;

namespace Nop.Services.Media.ElFinder;

/// <summary>
/// elFinder service interface
/// </summary>
public partial interface IElFinderService
{
    /// <summary>
    /// Configure elFinder connector
    /// </summary>
    /// <param name="storeId">Store identifier</param>
    /// <returns>Connector</returns>
    Connector GetConnector(int storeId);
}