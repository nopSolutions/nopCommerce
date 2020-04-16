using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Data;

namespace Nop.Tests
{
    public class FakeRepository<T> : Mock<IRepository<T>>, IFakeRepository<T> where T : BaseEntity
    {
        private readonly int[] _initIds = Array.Empty<int>();

        private readonly HashSet<T> _table = new HashSet<T>(new BaseEntityEqualityComparer<T>());

        public FakeRepository(IList<T> initData = null)
        {
            if (initData != null)
            {
                Insert(initData);
                _initIds = initData.Select(e => e.Id).ToArray();
            }

            SetupGRUD();
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

        protected T Insert(T entity)
        {
            SetNewId(entity);

            if (!_table.Add(entity))
                throw new ArgumentException($"Entity with id#{entity.Id} already exist");

            return entity;
        }

        protected IEnumerable<T> Insert(IList<T> entities)
        {
            if (entities is null)
                throw new ArgumentException(nameof(entities));

            foreach (var item in entities)
            {
                Insert(item);
            }

            return entities;
        }

        protected void Delete(T entity)
        {
            _table.Remove(entity);
        }

        protected void Delete(IEnumerable<T> entities)
        {
            if (entities is null)
                throw new ArgumentException(nameof(entities));

            foreach (var item in entities)
            {
                _table.Remove(item);
            }
        }

        private T GetById(object id)
        {
            return _table.FirstOrDefault(x => x.Id == Convert.ToInt32(id));
        }

        protected void SetupGRUD()
        {
            Setup(r => r.Table).Returns(_table.AsQueryable());

            Setup(r => r.Insert(It.IsAny<T>())).Callback((T value) => Insert(value));
            Setup(r => r.Insert(It.IsAny<IEnumerable<T>>())).Callback((IEnumerable<T> values) => Insert(values.ToList()));

            Setup(r => r.Delete(It.IsAny<T>())).Callback((T value) => Delete(value));
            Setup(r => r.Delete(It.IsAny<IEnumerable<T>>())).Callback((IEnumerable<T> values) => Delete(values));

            Setup(r => r.GetById(It.Is<int>(x => x > 0))).Returns((int id) => GetById(id));
        }

        public void ResetRepository()
        {
            _table.RemoveWhere(e => !_initIds.Contains(e.Id));
        }

        public IRepository<T> GetRepository()
        {
            return Object;
        }
    }
}
