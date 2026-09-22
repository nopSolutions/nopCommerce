using FluentAssertions;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Discounts;

[TestFixture]
public class DiscountServiceTests : ServiceTest
{
    private IDiscountPluginManager _discountPluginManager;
    private IDiscountService _discountService;

    [OneTimeSetUp]
    public void SetUp()
    {
        _discountPluginManager = GetService<IDiscountPluginManager>();
        _discountService = GetService<IDiscountService>();
    }

    [Test]
    public async Task CanGetAllDiscount()
    {
        var discounts = await _discountService.GetAllDiscountsAsync(isActive: null);
        discounts.Should().NotBeNull();
        discounts.Any().Should().BeTrue();
    }

    [Test]
    public async Task CanLoadDiscountRequirementRules()
    {
        var rules = await _discountPluginManager.LoadAllPluginsAsync();
        rules.Should().NotBeNull();
        rules.Any().Should().BeTrue();
    }

    [Test]
    public async Task CanLoadDiscountRequirementRuleBySystemKeyword()
    {
        var rule = await _discountPluginManager.LoadPluginBySystemNameAsync("TestDiscountRequirementRule");
        rule.Should().NotBeNull();
    }

    [Test]
    public async Task ShouldAcceptValidDiscountCode()
    {
        var discount = CreateDiscount();
        var customer = CreateCustomer();

        var result = await _discountService.ValidateDiscountAsync(discount, customer, ["CouponCode 1"]);
        result.IsValid.Should().BeTrue();
    }

    private static Customer CreateCustomer()
    {
        return new Customer
        {
            CustomerGuid = Guid.NewGuid(),
            AdminComment = string.Empty,
            Active = true,
            Deleted = false,
            CreatedOnUtc = new DateTime(2010, 01, 01),
            LastActivityDateUtc = new DateTime(2010, 01, 02)
        };
    }

    private static Discount CreateDiscount()
    {
        return new Discount
        {
            IsActive = true,
            DiscountType = DiscountType.AssignedToSkus,
            Name = "Discount 2",
            UsePercentage = false,
            DiscountPercentage = 0,
            DiscountAmount = 5,
            RequiresCouponCode = true,
            CouponCode = "CouponCode 1",
            DiscountLimitation = DiscountLimitationType.Unlimited
        };
    }

    [Test]
    public async Task ShouldNotAcceptWrongDiscountCode()
    {
        var discount = CreateDiscount();
        var customer = CreateCustomer();

        var result = await _discountService.ValidateDiscountAsync(discount, customer, ["CouponCode 2"]);
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public async Task CanValidateDiscountDateRange()
    {
        var discount = CreateDiscount();
        discount.RequiresCouponCode = false;

        var customer = CreateCustomer();

        var result = await _discountService.ValidateDiscountAsync(discount, customer, null);
        result.IsValid.Should().BeTrue();

        discount.StartDateUtc = DateTime.UtcNow.AddDays(1);
        result = await _discountService.ValidateDiscountAsync(discount, customer, null);
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void CanCalculateDiscountAmountPercentage()
    {
        var discount = new Discount
        {
            UsePercentage = true,
            DiscountPercentage = 30
        };

        _discountService.GetDiscountAmount(discount, 100).Should().Be(30);

        discount.DiscountPercentage = 60;
        _discountService.GetDiscountAmount(discount, 200).Should().Be(120);
    }

    [Test]
    public void CanCalculateDiscountAmountFixed()
    {
        var discount = new Discount
        {
            UsePercentage = false,
            DiscountAmount = 10
        };

        _discountService.GetDiscountAmount(discount, 100).Should().Be(10);

        discount.DiscountAmount = 20;
        _discountService.GetDiscountAmount(discount, 200).Should().Be(20);
    }

    [Test]
    public void MaximumDiscountAmountIsUsed()
    {
        var discount = new Discount
        {
            UsePercentage = true,
            DiscountPercentage = 30,
            MaximumDiscountAmount = 3.4M
        };

        _discountService.GetDiscountAmount(discount, 100).Should().Be(3.4M);

        discount.DiscountPercentage = 60;
        _discountService.GetDiscountAmount(discount, 200).Should().Be(3.4M);
        _discountService.GetDiscountAmount(discount, 100).Should().Be(3.4M);

        discount.DiscountPercentage = 1;
        discount.GetDiscountAmount(200).Should().Be(2);
    }

    [TestCase(RequirementGroupInteractionType.And)]
    [TestCase(RequirementGroupInteractionType.Or)]
    [TestCase(RequirementGroupInteractionType.And, true)]
    [TestCase(RequirementGroupInteractionType.Or, true)]
    [Test]
    public async Task CanCheckRequirements(RequirementGroupInteractionType interactionType, bool checkFalse = false)
    {
        var discount = CreateDiscount();
        discount.RequiresCouponCode = false;
        var customer = CreateCustomer();

        await _discountService.InsertDiscountAsync(discount);

        var groupRequirement = new DiscountRequirement
        {
            DiscountId = discount.Id,
            IsGroup = true,
            DiscountRequirementRuleSystemName = "TestDiscountRequirementRule",
            InteractionType = interactionType
        };

        await _discountService.InsertDiscountRequirementAsync(groupRequirement);

        var requirement1 = new DiscountRequirement
        {
            DiscountId = discount.Id,
            DiscountRequirementRuleSystemName = "TestDiscountRequirementRule",
            ParentId = groupRequirement.Id
        };

        var requirement2 = new DiscountRequirement
        {
            DiscountId = discount.Id,
            DiscountRequirementRuleSystemName = "TestDiscountRequirementRule",
            ParentId = groupRequirement.Id
        };

        if (interactionType == RequirementGroupInteractionType.Or || checkFalse)
        {
            requirement1.InteractionType = RequirementGroupInteractionType.And;

            if (interactionType == RequirementGroupInteractionType.Or && checkFalse)
                requirement2.InteractionType = RequirementGroupInteractionType.Or;
        }

        await _discountService.InsertDiscountRequirementAsync(requirement1);
        await _discountService.InsertDiscountRequirementAsync(requirement2);

        try
        {
            var result = await _discountService.ValidateDiscountAsync(discount, customer, null);
            result.IsValid.Should().Be(!checkFalse);

        }
        finally
        {
            await _discountService.DeleteDiscountRequirementAsync(groupRequirement, true);
            await _discountService.DeleteDiscountAsync(discount);
        }
    }

    [Test]
    public async Task CanGetAppliedDiscountsForManyEntitiesWithOneQueryAndSeedThePerRequestCache()
    {
        var productService = GetService<IProductService>();
        var product1 = await productService.GetProductByIdAsync(1);
        var product2 = await productService.GetProductByIdAsync(2);
        var discount = new Discount
        {
            Name = "Batch applied discounts test",
            DiscountType = DiscountType.AssignedToSkus,
            UsePercentage = true,
            DiscountPercentage = 10
        };
        await _discountService.InsertDiscountAsync(discount);
        var mapping = new DiscountProductMapping { DiscountId = discount.Id, EntityId = product1.Id };
        await productService.InsertDiscountProductMappingAsync(mapping);

        try
        {
            var applied = await _discountService.GetAppliedDiscountsAsync(new[] { product1, product2 });

            applied[product1.Id].Should().ContainSingle(d => d.Id == discount.Id);
            applied[product2.Id].Should().BeEmpty("entities without discounts are cached as empty lists");

            //the per-entity overload must be served from the seeded cache: same instances, no new query
            (await _discountService.GetAppliedDiscountsAsync(product1)).Should().BeSameAs(applied[product1.Id]);
            (await _discountService.GetAppliedDiscountsAsync(product2)).Should().BeSameAs(applied[product2.Id]);
        }
        finally
        {
            await productService.DeleteDiscountProductMappingAsync(mapping);
            await _discountService.DeleteDiscountAsync(discount);
        }
    }

    [Test]
    public async Task GetAppliedDiscountsForManyEntitiesIsServedFromTheCacheAndSkipsNullEntities()
    {
        var productService = GetService<IProductService>();
        var product = await productService.GetProductByIdAsync(1);
        try
        {
            var single = await _discountService.GetAppliedDiscountsAsync(product);

            //already cached entities are returned from the per-request cache, i.e. the very same instances
            (await _discountService.GetAppliedDiscountsAsync(new[] { product, null, product }))
                .Should().ContainSingle().Which.Value.Should().BeSameAs(single);

            (await _discountService.GetAppliedDiscountsAsync(Array.Empty<Product>())).Should().BeEmpty();
        }
        finally
        {
            GetService<IShortTermCacheManager>().Remove(NopDiscountDefaults.AppliedDiscountsCacheKey.Key, nameof(Product), product);
        }
    }
}

public static class DiscountExtensions
{
    private static readonly DiscountService _discountService;

    static DiscountExtensions()
    {
        _discountService = new DiscountService(null, null,
            null, null, null, null, null, null, null, null, null);
    }

    public static decimal GetDiscountAmount(this Discount discount, decimal amount)
    {
        ArgumentNullException.ThrowIfNull(discount);

        return _discountService.GetDiscountAmount(discount, amount);

    }
}
