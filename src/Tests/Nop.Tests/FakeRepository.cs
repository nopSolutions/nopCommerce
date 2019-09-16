using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Core.Data;

namespace Nop.Tests
{
    public class FakeRepository<T> : Mock<IRepository<T>>, IFakeRepository<T> where T : BaseEntity
    {
        private int[] _initIds = new int[] { };

        public HashSet<T> Table = new HashSet<T>(new BaseEntityEqualityComparer<T>());

        public FakeRepository(IEnumerable<T> initData = null)
        {
            if (initData != null)
            {
                Insert(initData);
                _initIds = initData.Select(e=>e.Id).ToArray();
            }
            

            SetupGRUD();
        }

        private void SetNewId(T entity)
        {
            if (entity.Id != 0)
                return;

            if (Table.Count == 0)
            {
                entity.Id = 1;
                return;
            }

            entity.Id = Table.Max(e => e.Id) + 1;
        }

        protected T Insert(T entity)
        {
            SetNewId(entity);

            if (!Table.Add(entity))
                throw new ArgumentException($"Entity with id#{entity.Id} already exist");

            return entity;
        }

        protected IEnumerable<T> Insert(IEnumerable<T> entities)
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
            Table.Remove(entity);
        }

        protected void Delete(IEnumerable<T> entities)
        {
            if (entities is null)
                throw new ArgumentException(nameof(entities));

            foreach (var item in entities)
            {
                Table.Remove(item);
            }
        }

        protected T GetById(int id)
        {
            return Table.FirstOrDefault(x => x.Id == id);
        }

        protected void SetupGRUD()
        {
            Setup(r => r.Table).Returns(Table.AsQueryable());

            Setup(r => r.Insert(It.IsAny<T>())).Callback((T value) => Insert(value));
            Setup(r => r.Insert(It.IsAny<IEnumerable<T>>())).Callback((IEnumerable<T> values) => Insert(values));

            Setup(r => r.Delete(It.IsAny<T>())).Callback((T value) => Delete(value));
            Setup(r => r.Delete(It.IsAny<IEnumerable<T>>())).Callback((IEnumerable<T> values) => Delete(values));

            Setup(r => r.GetById(It.Is<int>(x => x > 0))).Returns((int id) => GetById(id));
        }

        public void ResetRepository()
        {
            Table.RemoveWhere(e => !_initIds.Contains(e.Id));
        }

        public IRepository<T> GetRepository()
        {
            return Object;
        }
    }
}
