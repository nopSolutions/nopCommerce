using elFinder.NetCore;
using Microsoft.AspNetCore.Http;

namespace Nop.Services.Media.ElFinder;

/// <summary>
/// elFinder service interface
/// </summary>
public partial interface IElFinderService
{
    /// <summary>
    /// Configure elFinder connector
    /// </summary>
    /// <param name="request">Http request</param>
    /// <returns>Connector</returns>
    Task<Connector> GetConnectorAsync(HttpRequest request);

}
