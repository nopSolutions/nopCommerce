using Nop.Plugin.CustomAPI.Repository.IRepository;

namespace Nop.Plugin.CustomAPI.Repository
{
    public class UnitOfWork : IUnitOfWork
    {

        public UnitOfWork(IProductRepository productRepository)
        {
            this.ProductRepository = productRepository;
        }

        public IProductRepository ProductRepository { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            
        }
    }
}
