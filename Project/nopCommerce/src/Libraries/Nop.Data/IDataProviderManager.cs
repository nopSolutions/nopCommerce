namespace Nop.Data;

/// <summary>
/// Represents a data provider manager
/// </summary>
public partial interface IDataProviderManager
{
    #region Properties

    /// <summary>
    /// Gets data provider
    /// </summary>
    INopDataProvider DataProvider { get; }

    #endregion
}