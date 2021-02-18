using FluentAssertions;
using Nop.Core.Domain.Configuration;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Domain.Configuration
{
    [TestFixture]
    public class SettingTestFixture
    {
        [Test]
        public void CanCreateSetting()
        {
            var setting = new Setting("Setting1", "Value1");
            setting.Name.Should().Be("Setting1");
            setting.Value.Should().Be("Value1");
        }
    }
}
