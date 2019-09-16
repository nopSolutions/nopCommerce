using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Nop.Core;
using Nop.Core.Data;

namespace Nop.Tests
{
    public static class FakePrepareExtensions
    {
        public static IRepository<T> FakeRepoNullPropagation<T>(this IRepository<T> repository) where T : BaseEntity
        {
            if (repository is null)
                return new Mock<IRepository<T>>().Object;
            else
                return repository;
                    
        }
    }
}
