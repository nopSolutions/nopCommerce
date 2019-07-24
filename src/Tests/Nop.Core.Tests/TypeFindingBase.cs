using System;
using Nop.Core.Infrastructure;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests
{
    public abstract class TypeFindingBase : TestsBase
    {
        protected ITypeFinder typeFinder;

        protected abstract Type[] GetTypes();

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            typeFinder = new Fakes.FakeTypeFinder(typeof(TypeFindingBase).Assembly, GetTypes());
        }
    }
}
