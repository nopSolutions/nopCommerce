using Nop.Core;
using Nop.Core.Data;

namespace Nop.Tests
{
    public interface IFakeRepository<T>: IFakeStoreRepositoryContainer where T : BaseEntity
    {
        IRepository<T> GetRepository();
    }
}
