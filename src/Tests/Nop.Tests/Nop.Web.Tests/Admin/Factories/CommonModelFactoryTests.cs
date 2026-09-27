using AwesomeAssertions;
using Nop.Web.Areas.Admin.Factories;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Factories;

[TestFixture]
public class CommonModelFactoryTests : BaseNopTest
{
    private TestCommonModelFactory _commonModelFactory;

    [OneTimeSetUp]
    public void SetUp()
    {
        _commonModelFactory = (TestCommonModelFactory)GetService<ICommonModelFactory>();
    }

    [Test]
    public async Task TestGetNopLatestVersion()
    {
        var nopLatestVersion = await _commonModelFactory.GetNopLatestVersionAsync();
        nopLatestVersion.Should().NotBeNullOrEmpty();
    }
}
