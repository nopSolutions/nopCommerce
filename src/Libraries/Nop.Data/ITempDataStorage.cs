using System;
using System.Linq;

namespace Nop.Data
{
    public interface ITempDataStorage<T> : IQueryable<T>, IDisposable where T : class
    {

    }
}