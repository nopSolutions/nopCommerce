﻿using Nop.Core.Domain.Stores;

namespace Nop.Core;

/// <summary>
/// Store context
/// </summary>
public partial interface IStoreContext
{
    /// <summary>
    /// Gets the current store
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<Store> GetCurrentStoreAsync();

    /// <summary>
    /// Gets active store scope configuration
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<int> GetActiveStoreScopeConfigurationAsync();
}