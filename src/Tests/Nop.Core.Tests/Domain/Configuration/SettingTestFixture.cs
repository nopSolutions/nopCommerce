using Nop.Core.Domain.Configuration;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests.Domain.Configuration
{
    [TestFixture]
    public class SettingTestFixture
    {
        [Test]
        public void Can_create_setting()
        {
            var setting = new Setting("Setting1", "Value1");
            setting.Name.ShouldEqual("Setting1");
            setting.Value.ShouldEqual("Value1");
        }
    }
}
