using AutoMapper;
using NUnit.Framework;
using Nop.Admin.Infrastructure;

namespace Nop.Web.MVC.Tests.Admin.Infrastructure
{
    [TestFixture]
    public class AutoMapperConfigurationTest
    {
        [Test]
        public void Configuration_is_valid()
        {
            var autoMapperStartupTask = new AutoMapperStartupTask();
            autoMapperStartupTask.Execute();
            Mapper.AssertConfigurationIsValid();
        }
    }
}
