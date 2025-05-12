using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Web.Factories;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class NewsletterModelFactoryTests : BaseNopTest
{
    private INewsLetterModelFactory _newsletterModelFactory;

    [OneTimeSetUp]
    public void SetUp()
    {
        _newsletterModelFactory = GetService<INewsLetterModelFactory>();
    }

    [Test]
    public async Task CanPrepareNewsletterBoxModel()
    {
        var model = await _newsletterModelFactory.PrepareNewsLetterBoxModelAsync();

        model.AllowToUnsubscribe.Should().Be(GetService<CustomerSettings>().NewsletterBlockAllowToUnsubscribe);
    }

    [Test]
    public async Task CanPrepareSubscriptionActivationModel()
    {
        var activated = (await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(true)).Result;

        var deactivated = (await _newsletterModelFactory.PrepareSubscriptionActivationModelAsync(false)).Result;

        activated.Should().NotBe(deactivated);
    }
}