using Nop.Data;

namespace Nop.Tests
{
    /// <summary>
    /// Represents the data provider manager
    /// </summary>
    public partial class TestDataProviderManager : IDataProviderManager
    {
        #region Properties

        /// <summary>
        /// Gets data provider
        /// </summary>
        public INopDataProvider DataProvider => new SqLiteNopDataProvider();

        #endregion
    }
}
