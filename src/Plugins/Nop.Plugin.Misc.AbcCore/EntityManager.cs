using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore
{
    /// <summary>
    /// A helper class to manage bulk inserts and updates
    /// </summary>
    public class EntityManager<T> where T : BaseEntity
    {
        private IRepository<T> _repo;
        private readonly int _bufferSize;
        private IList<T> _insertBuffer;
        private IList<T> _updateBuffer;

        public EntityManager()
        {
            _repo = EngineContext.Current.Resolve<IRepository<T>>();
            _bufferSize = 500;
            _insertBuffer = new List<T>();
            _updateBuffer = new List<T>();
        }

        public EntityManager(IRepository<T> repo)
        {
            _repo = repo;
            _bufferSize = 500;
            _insertBuffer = new List<T>();
            _updateBuffer = new List<T>();

        }

        public async Task InsertAsync(T entity)
        {
            _insertBuffer.Add(entity);

            if (_insertBuffer.Count >= _bufferSize)
            {
                await _repo.InsertAsync(_insertBuffer);
                _insertBuffer.Clear();
            }
        }

        public async Task UpdateAsync(T entity)
        {
            _updateBuffer.Add(entity);

            if (_updateBuffer.Count >= _bufferSize)
            {
                await _repo.UpdateAsync(_updateBuffer);
                _updateBuffer.Clear();
            }
        }

        public async Task FlushAsync()
        {
            await _repo.InsertAsync(_insertBuffer);
            await _repo.UpdateAsync(_updateBuffer);
            _insertBuffer.Clear();
            _updateBuffer.Clear();
        }
    }
}
