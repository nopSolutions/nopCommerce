using LinqToDB.Data;
using LinqToDB.Mapping;

namespace Nop.Data
{
    /// <summary>
    /// Implements database connection abstraction.
    /// </summary>
    internal class NopDataConnection : DataConnection
    {
        #region Ctor

        public NopDataConnection()
        {
            AddMappingSchema(AdditionalSchema);
        }

        #endregion

        #region Properties

        public static MappingSchema AdditionalSchema { get; } = new MappingSchema();

        #endregion
    }
}
