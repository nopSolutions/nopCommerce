
namespace Nop.Core.Data
{
    public interface IDataProvider
    {
        void InitDatabase();

        bool StoredProceduredSupported { get; }
    }
}
