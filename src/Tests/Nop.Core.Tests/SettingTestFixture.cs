using Nop.Core.Domain.Configuration;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Core.Tests
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

        [Test]
        public void Can_get_typed_setting_value()
        {
            var setting = new Setting("IntSetting", "1000");
            setting.As<int>().ShouldBe<int>();
            setting.As<int>().ShouldEqual(1000);
        }
    }
}
