using FluentAssertions;
using Nop.Core.Domain.Affiliates;
using Nop.Services.Affiliates;
using Nop.Services.Common;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Affiliates;

[TestFixture]
public class AffiliateServiceTests : ServiceTest<Affiliate>
{
    private IAffiliateService _affiliateService;
    private IAddressService _addressService;
    private Affiliate _activeAffiliate1;
    private Affiliate _activeAffiliate2;
    private Affiliate _notActiveAffiliate;
    private Affiliate _activeDeletedAffiliate;
    private Affiliate _notActiveDeletedAffiliate;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        _affiliateService = GetService<IAffiliateService>();
        _addressService = GetService<IAddressService>();
        _activeAffiliate1 = new Affiliate
        {
            Active = true,
            AddressId = 1,
            AdminComment = "Test admin comment",
            FriendlyUrlName = "TestActiveAffiliate1"
        };
        _activeAffiliate2 = new Affiliate
        {
            Active = true,
            AddressId = 1,
            AdminComment = "Test admin comment",
            FriendlyUrlName = "TestActiveAffiliate2"
        };
        _notActiveAffiliate = new Affiliate
        {
            Active = false,
            AddressId = 1,
            AdminComment = "Test admin comment",
            FriendlyUrlName = "TestNotActiveAffiliate"
        };
        _activeDeletedAffiliate = new Affiliate
        {
            Active = true,
            AddressId = 1,
            AdminComment = "Test admin comment",
            FriendlyUrlName = "TestActiveDeletedAffiliate",
            Deleted = true
        };
        _notActiveDeletedAffiliate = new Affiliate
        {
            Active = false,
            AddressId = 1,
            AdminComment = "Test admin comment",
            FriendlyUrlName = "TestNotActiveDeletedAffiliate",
            Deleted = true
        };

        await _affiliateService.InsertAffiliateAsync(_activeAffiliate1);
        await _affiliateService.InsertAffiliateAsync(_notActiveAffiliate);
        await _affiliateService.InsertAffiliateAsync(_activeDeletedAffiliate);
        await _affiliateService.InsertAffiliateAsync(_notActiveDeletedAffiliate);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _affiliateService.DeleteAffiliateAsync(_activeAffiliate1);
        await _affiliateService.DeleteAffiliateAsync(_notActiveAffiliate);
        await _affiliateService.DeleteAffiliateAsync(_activeDeletedAffiliate);
        await _affiliateService.DeleteAffiliateAsync(_notActiveDeletedAffiliate);
    }

    [Test]
    public async Task CanGetAffiliateByFriendlyUrlName()
    {
        var affiliate = await _affiliateService.GetAffiliateByFriendlyUrlNameAsync(_activeAffiliate1.FriendlyUrlName);

        affiliate.Should().NotBeNull();
        affiliate.Active.Should().BeTrue();
        affiliate.FriendlyUrlName.Should().Be(_activeAffiliate1.FriendlyUrlName);
    }

    [Test]
    public async Task CanGetAllAffiliates()
    {
        var affiliates = await _affiliateService.GetAllAffiliatesAsync();
        affiliates.TotalCount.Should().Be(2);
        affiliates = await _affiliateService.GetAllAffiliatesAsync(showHidden: true);
        affiliates.TotalCount.Should().Be(3);
        affiliates = await _affiliateService.GetAllAffiliatesAsync(_activeDeletedAffiliate.FriendlyUrlName);
        affiliates.TotalCount.Should().Be(0);
        affiliates = await _affiliateService.GetAllAffiliatesAsync(_activeAffiliate1.FriendlyUrlName, showHidden: true);
        affiliates.TotalCount.Should().Be(1);

        var address = await _addressService.GetAddressByIdAsync(1);

        affiliates = await _affiliateService.GetAllAffiliatesAsync(firstName: address.FirstName, showHidden: true);
        affiliates.TotalCount.Should().Be(3);
        affiliates = await _affiliateService.GetAllAffiliatesAsync(firstName: address.FirstName);
        affiliates.TotalCount.Should().Be(2);
        affiliates = await _affiliateService.GetAllAffiliatesAsync(loadOnlyWithOrders: true, showHidden: true);
        affiliates.TotalCount.Should().Be(0);
    }

    [Test]
    public async Task CanGetAffiliateFullName()
    {
        var fullName = await _affiliateService.GetAffiliateFullNameAsync(_activeAffiliate1);
        var address = await _addressService.GetAddressByIdAsync(1);

        fullName.Should().Be($"{address.FirstName} {address.LastName}");
    }

    [Test]
    public async Task CanGenerateUrl()
    {
        var url = await _affiliateService.GenerateUrlAsync(_activeAffiliate1);
        url.Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?{NopAffiliateDefaults.AffiliateQueryParameter}={_activeAffiliate1.FriendlyUrlName}");

        _notActiveDeletedAffiliate.FriendlyUrlName = string.Empty;
        url = await _affiliateService.GenerateUrlAsync(_notActiveDeletedAffiliate);
        url.Should().Be($"http://{NopTestsDefaults.HostIpAddress}/?{NopAffiliateDefaults.AffiliateIdQueryParameter}={_notActiveDeletedAffiliate.Id}");
    }

    [Test]
    public async Task CanValidateFriendlyUrlName()
    {
        var friendlyUrlName = await _affiliateService.ValidateFriendlyUrlNameAsync(_activeAffiliate1, _activeAffiliate2.FriendlyUrlName);
        friendlyUrlName.Should().Be(_activeAffiliate2.FriendlyUrlName.ToLowerInvariant());

        friendlyUrlName = await _affiliateService.ValidateFriendlyUrlNameAsync(_activeAffiliate1, "not/valid/url*name");
        friendlyUrlName.Should().Be("notvalidurlname");
    }

    protected override CrudData<Affiliate> CrudData
    {
        get
        {
            var baseEntity = new Affiliate
            {
                Active = true,
                AddressId = 1,
                AdminComment = "Test admin comment",
                FriendlyUrlName = "TestActiveAffiliate"
            };

            var updatedEntity = new Affiliate
            {
                Active = true, AddressId = 1, AdminComment = "Test comment", FriendlyUrlName = "TestActiveAffiliate"
            };

            return new CrudData<Affiliate>
            {
                BaseEntity = baseEntity,
                UpdatedEntity = updatedEntity,
                Insert = _affiliateService.InsertAffiliateAsync,
                Update = _affiliateService.UpdateAffiliateAsync,
                Delete = _affiliateService.DeleteAffiliateAsync,
                GetById = _affiliateService.GetAffiliateByIdAsync,
                IsEqual = (first, second) => first.Active == second.Active && first.AddressId == second.AddressId && first.AdminComment.Equals(second.AdminComment) && first.FriendlyUrlName.Equals(second.FriendlyUrlName) && first.Deleted == second.Deleted
            };
        }
    }

}