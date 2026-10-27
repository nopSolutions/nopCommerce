using System.Linq.Expressions;
using AwesomeAssertions;
using Nop.Services;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests;

[TestFixture]
public class GetCachedCompiledDelegateTests
{
    private sealed class FakeEntity
    {
        public string Name { get; init; } = string.Empty;
        public string ShortName { get; init; } = string.Empty;
        public int Id { get; set; }
    }

    private sealed class OtherFakeEntity
    {
        public string Name => string.Empty;
    }

    [Test]
    public void GetCompiledReturnsSameDelegateForSameProperty()
    {
        Expression<Func<FakeEntity, string>> sel1 = x => x.Name;
        Expression<Func<FakeEntity, string>> sel2 = x => x.Name;

        var d1 = sel1.GetCompiled();
        var d2 = sel2.GetCompiled();

        d1.Should().BeSameAs(d2);
    }

    [Test]
    public void GetCompiledReturnsDifferentDelegateForDifferentProperty()
    {
        Expression<Func<FakeEntity, string>> sel1 = x => x.Name;
        Expression<Func<FakeEntity, string>> sel2 = x => x.ShortName;

        var dName = sel1.GetCompiled();
        var dShort = sel2.GetCompiled();

        dName.Should().NotBeSameAs(dShort);
    }

    [Test]
    public void GetCompiledReturnsDifferentDelegateForDifferentEntityType()
    {
        Expression<Func<FakeEntity, string>> sel1 = x => x.Name;
        Expression<Func<OtherFakeEntity, string>> sel2 = x => x.Name;

        var d1 = sel1.GetCompiled();
        var d2 = sel2.GetCompiled();

        d1.Should().NotBeSameAs(d2);
    }

    [Test]
    public void GetCompiledDoesNotCollideOnDifferentPropertyType()
    {
        Expression<Func<FakeEntity, string>> sel1 = x => x.Name;
        Expression<Func<FakeEntity, int>> sel2 = x => x.Id;

        var dName = sel1.GetCompiled();
        var dId = sel2.GetCompiled();

        dName.Should().NotBeSameAs(dId);
    }

    [Test]
    public void GetCompiledFallsBackForNonPropertySelector()
    {
        Expression<Func<FakeEntity, string>> selector = x => x.Name.ToUpperInvariant();
        var entity = new FakeEntity { Name = "abc" };

        var compiled = selector.GetCompiled();

        compiled(entity).Should().Be("ABC");
    }

    [Test]
    public void GetCompiledReturnsCorrectValue()
    {
        var entity = new FakeEntity { Name = "Casio", ShortName = "CAS" };

        Expression<Func<FakeEntity, string>> sel1 = x => x.Name;
        Expression<Func<FakeEntity, string>> sel2 = x => x.ShortName;

        var nameGetter = sel1.GetCompiled();
        var shortGetter = sel2.GetCompiled();

        nameGetter(entity).Should().Be("Casio");
        shortGetter(entity).Should().Be("CAS");
    }
}
