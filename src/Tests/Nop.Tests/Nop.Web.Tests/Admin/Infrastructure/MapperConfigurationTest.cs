using Nop.Web.Areas.Admin.Infrastructure.Mapper;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Infrastructure;

[TestFixture]
public class MapperConfigurationTest
{
    [Test]
    public void MapperCanBeInstantiated()
    {
        // Since Mapperly validates mappings at compile time, we just check that the mapper can be instantiated
        var mapper = new AdminMapper();
        Assert.That(mapper, Is.Not.Null);
    }
}