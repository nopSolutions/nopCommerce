using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB.Data;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;

namespace Nop.Tests
{
    public class FakeRepository<T> : IRepository<T>, IFakeRepository<T> where T: BaseEntity
    {
        private readonly int[] _initIds = Array.Empty<int>();

        private readonly HashSet<T> _table = new HashSet<T>(new BaseEntityEqualityComparer<T>());

        public FakeRepository(IList<T> initData = null)
        {
            if (initData == null) 
                return;

            Insert(initData);
            _initIds = initData.Select(e => e.Id).ToArray();
        }

        private void SetNewId(T entity)
        {
            if (entity.Id != 0)
                return;

            if (_table.Count == 0)
            {
                entity.Id = 1;
                return;
            }

            entity.Id = _table.Max(e => e.Id) + 1;
        }
       
        public void Insert(T entity, bool publishEvent = true)
        {
            SetNewId(entity);

            if (!_table.Add(entity))
                throw new ArgumentException($"Entity with id#{entity.Id} already exist");
        }

        public void Insert(IList<T> entities, bool publishEvent = true)
        {
            if (entities is null)
                throw new ArgumentException(nameof(entities));

            foreach (var item in entities) 
                Insert(item);
        }

        public T LoadOriginalCopy(T entity)
        {
            return entity;
        }

        public void Update(T entity, bool publishEvent = true)
        {
            Delete(_table.FirstOrDefault(x => x.Id == entity.Id));
            Insert(entity);
        }

        public void Update(IList<T> entities, bool publishEvent = true)
        {
            Delete(_table.Where(x => entities.Any(p=>p.Id==x.Id)).ToList());
            Insert(entities);
        }

        public void Delete(T entity, bool publishEvent = true)
        {
            _table.Remove(entity);
        }

        public void Delete(IList<T> entities, bool publishEvent = true)
        {
            if (entities is null)
                throw new ArgumentException(nameof(entities));

            foreach (var item in entities) 
                _table.Remove(item);
        }

        public void Delete(Expression<Func<T, bool>> predicate)
        {
            var foo = predicate.Compile();
            Delete(_table.ToList().Where(p=>foo(p)).ToList());
        }

        public IList<T> EntityFromSql(string storeProcedureName, params DataParameter[] dataParameters)
        {
            throw new NotImplementedException();
        }

        public void Truncate(bool resetIdentity = false)
        {
            throw new NotImplementedException();
        }
        
        public T GetById(int? id, Func<IStaticCacheManager, CacheKey> getCacheKey = null)
        {
            return _table.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
        }

        public IList<T> GetByIds(IList<int> ids, Func<IStaticCacheManager, CacheKey> getCacheKey = null)
        {
            return _table.Where(p => ids.Contains(p.Id)).ToList();
        }

        public IList<T> GetAll(Func<IQueryable<T>, IQueryable<T>> func = null, Func<IStaticCacheManager, CacheKey> getCacheKey = null)
        {
            var query = _table.AsQueryable();
            if (func != null)
                query = func.Invoke(query);

            return query.ToList();
        }

        public IPagedList<T> GetAllPaged(Func<IQueryable<T>, IQueryable<T>> func = null, int pageIndex = 0, int pageSize = int.MaxValue,
            bool getOnlyTotalCount = false)
        {
            return new PagedList<T>(GetAll(func), pageIndex, pageSize);
        }

        public IQueryable<T> Table => _table.AsQueryable();

        public IRepository<T> GetRepository()
        {
            return this;
        }

        public void ResetRepository()
        {
            _table.RemoveWhere(e => !_initIds.Contains(e.Id));
        }
    }
}
