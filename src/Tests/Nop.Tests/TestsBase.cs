using System.Security.Principal;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Tests
{
    public abstract class TestsBase
    {
        protected MockRepository mocks;

        [SetUp]
        public virtual void SetUp()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (mocks != null)
            {
                mocks.ReplayAll();
                mocks.VerifyAll();
            }
        }

        protected static IPrincipal CreatePrincipal(string name, params string[] roles)
        {
            return new GenericPrincipal(new GenericIdentity(name, "TestIdentity"), roles);
        }
    }
}
