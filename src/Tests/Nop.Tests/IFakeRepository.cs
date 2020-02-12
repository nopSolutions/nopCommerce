using Nop.Core;
using Nop.Data;

namespace Nop.Tests
{
    public interface IFakeRepository<T>: IFakeStoreRepositoryContainer where T : BaseEntity
    {
        IRepository<T> GetRepository();
    }
}
