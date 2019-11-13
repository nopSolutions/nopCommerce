using Moq;
using Nop.Core;
using Nop.Data;

namespace Nop.Tests
{
    public static class FakePrepareExtensions
    {
        public static IRepository<T> FakeRepoNullPropagation<T>(this IRepository<T> repository) where T : BaseEntity
        {
            return repository ?? new Mock<IRepository<T>>().Object;
        }
    }
}
