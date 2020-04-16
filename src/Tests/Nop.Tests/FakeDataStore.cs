using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Data;

namespace Nop.Tests
{
    public class FakeDataStore
    {
        private readonly IDictionary<Type, IFakeStoreRepositoryContainer> _store;

        public FakeDataStore()
        {
            _store = new Dictionary<Type, IFakeStoreRepositoryContainer>();
        }

        public virtual IRepository<T> GetRepository<T>() where T : BaseEntity
        {
            if (IsExistsRepo<T>() && _store[typeof(T)] is IFakeRepository<T> repo)
                return repo.GetRepository();

            return null;
        }

        public virtual IRepository<T> RegRepository<T>(IFakeRepository<T> repositoryWizard) where T : BaseEntity
        {
            if (repositoryWizard is null)
                throw new ArgumentNullException(nameof(repositoryWizard));

            if (IsExistsRepo<T>())
                throw new ArgumentException($"{nameof(IRepository<T>)} already registered");

            _store.Add(typeof(T), repositoryWizard);

            return repositoryWizard.GetRepository();
        }

        public virtual IRepository<T> RegRepository<T>(IList<T> initData = null) where T : BaseEntity
        {
            return RegRepository(new FakeRepository<T>(initData));
        }

        public virtual void ResetStore()
        {
            foreach (var repo in _store)
            {
                if (repo.Value is IFakeStoreRepositoryContainer fakeRepository)
                    fakeRepository.ResetRepository();
                else
                    throw new ArrayTypeMismatchException();
            }
        }

        protected bool IsExistsRepo<T>() where T : BaseEntity
        {
            return _store.ContainsKey(typeof(T));
        }
    }
}
