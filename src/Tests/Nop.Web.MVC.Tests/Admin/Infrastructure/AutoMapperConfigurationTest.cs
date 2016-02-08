using AutoMapper;
using Nop.Admin.Infrastructure;
using Nop.Core.Infrastructure;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Admin.Infrastructure
{
    [TestFixture]
    public class AutoMapperConfigurationTest
    {
        [Test]
        public void Configuration_is_valid()
        {
            var autoMapper = EngineContext.Current.Resolve<AutoMapperInit>();
            autoMapper.configuration.AssertConfigurationIsValid();
        }
    }
}
