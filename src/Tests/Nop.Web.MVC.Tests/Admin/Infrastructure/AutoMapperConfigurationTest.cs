using Nop.Admin.Infrastructure.Mapper;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Admin.Infrastructure
{
    [TestFixture]
    public class AutoMapperConfigurationTest
    {
        [Test]
        public void Configuration_is_valid()
        {
            AutoMapperConfiguration.Init();
            AutoMapperConfiguration.MapperConfiguration.AssertConfigurationIsValid();
        }
    }
}
