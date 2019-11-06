using LinqToDB.Data;
using Nop.Core.Data;

namespace Nop.Data
{
    public class NopDataProvider: IDataProvider
    {
        public void InitializeDatabase()
        {
        }

        public DataParameter GetParameter()
        {
            return new DataParameter();
        }

        public bool BackupSupported { get; } = true;

        public int SupportedLengthOfBinaryHash { get; } = 8000;
    }
}
