using Nop.Plugin.CustomAPI.Repository.IRepository;

namespace Nop.Plugin.CustomAPI.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository ProductRepository { get; }
    }
}
