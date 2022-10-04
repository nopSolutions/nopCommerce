using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Vendors;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.Common;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories
{
    [TestFixture]
    public class CommonModelFactoryTests : BaseNopTest
    {
        private ICommonModelFactory _commonModelFactory;
        private LocalizationSettings _localizationSettings;
        private IWorkContext _workContext;
        private CustomerSettings _customerSettings;
        private ForumSettings _forumSettings;
        private StoreInformationSettings _storeInformationSettings;
        private NewsSettings _newsSettings;
        private CatalogSettings _catalogSettings;
        private DisplayDefaultFooterItemSettings _displayDefaultFooterItemSettings;
        private CommonSettings _commonSettings;
        private Vendor _vendor;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _commonModelFactory = GetService<ICommonModelFactory>();
            _localizationSettings = GetService<LocalizationSettings>();
            _workContext = GetService<IWorkContext>();
            _customerSettings = GetService<CustomerSettings>();
            _forumSettings = GetService<ForumSettings>();
            _storeInformationSettings = GetService<StoreInformationSettings>();
            _newsSettings = GetService<NewsSettings>();
            _catalogSettings = GetService<CatalogSettings>();
            _commonSettings = GetService<CommonSettings>();
            _displayDefaultFooterItemSettings = GetService<DisplayDefaultFooterItemSettings>();

            _vendor = await GetService<IVendorService>().GetVendorByIdAsync(1);
        }

        [Test]
        public async Task CanPrepareLogoModel()
        {
            var model = await _commonModelFactory.PrepareLogoModelAsync();
            model.StoreName.Should().NotBeNullOrEmpty();
            model.StoreName.Should().Be("Your store name");
            model.LogoPath.Should().NotBeNullOrEmpty();
            model.LogoPath.Should()
                .Be($"http://{NopTestsDefaults.HostIpAddress}/Themes/DefaultClean/Content/images/logo.png");
        }

        [Test]
        public async Task CanPrepareLanguageSelectorModel()
        {
            var model = await _commonModelFactory.PrepareLanguageSelectorModelAsync();

            model.CurrentLanguageId.Should().Be(1);
            model.UseImages.Should().Be(_localizationSettings.UseImagesForLanguageSelection);

            model.AvailableLanguages.Should().NotBeNullOrEmpty();
            var lang = model.AvailableLanguages.FirstOrDefault();
            lang.Should().NotBeNull();
            lang?.Name.Should().Be("EN");
            lang?.FlagImageFileName.Should().Be("us.png");
        }

        [Test]
        public async Task CanPrepareCurrencySelectorModel()
        {
            var model = await _commonModelFactory.PrepareCurrencySelectorModelAsync();
            model.CurrentCurrencyId.Should().Be(1);
            model.AvailableCurrencies.Should().NotBeNullOrEmpty();
            model.AvailableCurrencies.Count.Should().Be(1);
        }

        [Test]
        public async Task CanPrepareTaxTypeSelectorModel()
        {
            var model = await _commonModelFactory.PrepareTaxTypeSelectorModelAsync();
            model.CurrentTaxType.Should().Be(await _workContext.GetTaxDisplayTypeAsync());
        }

        [Test]
        public async Task CanPrepareHeaderLinksModel()
        {
            var model = await _commonModelFactory.PrepareHeaderLinksModelAsync();

            model.RegistrationType.Should().Be(_customerSettings.UserRegistrationType);
            model.IsAuthenticated.Should().BeTrue();
            model.CustomerName.Should().Be("John");
            model.ShoppingCartEnabled.Should().BeTrue();
            model.WishlistEnabled.Should().BeTrue();
            model.AllowPrivateMessages.Should().Be(_forumSettings.AllowPrivateMessages);
            model.UnreadPrivateMessages.Should().BeEmpty();
            model.AlertMessage.Should().BeEmpty();
            model.ShoppingCartItems.Should().Be(0);
            model.WishlistItems.Should().Be(0);
        }

        [Test]
        public async Task CanPrepareAdminHeaderLinksModel()
        {
            var model = await _commonModelFactory.PrepareAdminHeaderLinksModelAsync();
            model.ImpersonatedCustomerName.Should().Be("John");
            model.IsCustomerImpersonated.Should().BeFalse();
            model.DisplayAdminLink.Should().BeTrue();
            model.EditPageUrl.Should().BeNull();
        }

        [Test]
        public async Task CanPrepareSocialModel()
        {
            var model = await _commonModelFactory.PrepareSocialModelAsync();

            model.FacebookLink.Should().Be(_storeInformationSettings.FacebookLink);
            model.TwitterLink.Should().Be(_storeInformationSettings.TwitterLink);
            model.YoutubeLink.Should().Be(_storeInformationSettings.YoutubeLink);
            model.InstagramLink.Should().Be(_storeInformationSettings.InstagramLink);
            model.WorkingLanguageId.Should().Be(1);
            model.NewsEnabled.Should().Be(_newsSettings.Enabled);
        }

        [Test]
        public async Task CanPrepareFooterModel()
        {
            var model = await _commonModelFactory.PrepareFooterModelAsync();

            model.StoreName.Should().Be("Your store name");
            model.WishlistEnabled.Should().BeTrue();
            model.ShoppingCartEnabled.Should().BeTrue();
            model.SitemapEnabled.Should().BeTrue();
            model.SearchEnabled.Should().BeTrue();
            model.WorkingLanguageId.Should().Be(1);
            model.BlogEnabled.Should().BeTrue();
            model.CompareProductsEnabled.Should().Be(_catalogSettings.CompareProductsEnabled);
            model.ForumEnabled.Should().Be(_forumSettings.ForumsEnabled);
            model.NewsEnabled.Should().Be(_newsSettings.Enabled);
            model.RecentlyViewedProductsEnabled.Should().Be(_catalogSettings.RecentlyViewedProductsEnabled);
            model.NewProductsEnabled.Should().Be(_catalogSettings.NewProductsEnabled);
            model.DisplayTaxShippingInfoFooter.Should().Be(_catalogSettings.DisplayTaxShippingInfoFooter);
            model.HidePoweredByNopCommerce.Should().Be(_storeInformationSettings.HidePoweredByNopCommerce);
            model.AllowCustomersToApplyForVendorAccount.Should().BeTrue();
            model.AllowCustomersToCheckGiftCardBalance.Should().BeFalse();
            model.DisplaySitemapFooterItem.Should().Be(_displayDefaultFooterItemSettings.DisplaySitemapFooterItem);
            model.DisplayContactUsFooterItem.Should().Be(_displayDefaultFooterItemSettings.DisplayContactUsFooterItem);
            model.DisplayProductSearchFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayProductSearchFooterItem);
            model.DisplayNewsFooterItem.Should().Be(_displayDefaultFooterItemSettings.DisplayNewsFooterItem);
            model.DisplayBlogFooterItem.Should().Be(_displayDefaultFooterItemSettings.DisplayBlogFooterItem);
            model.DisplayForumsFooterItem.Should().Be(_displayDefaultFooterItemSettings.DisplayForumsFooterItem);
            model.DisplayRecentlyViewedProductsFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayRecentlyViewedProductsFooterItem);
            model.DisplayCompareProductsFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayCompareProductsFooterItem);
            model.DisplayNewProductsFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayNewProductsFooterItem);
            model.DisplayCustomerInfoFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayCustomerInfoFooterItem);
            model.DisplayCustomerOrdersFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayCustomerOrdersFooterItem);
            model.DisplayCustomerAddressesFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayCustomerAddressesFooterItem);
            model.DisplayShoppingCartFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayShoppingCartFooterItem);
            model.DisplayWishlistFooterItem.Should().Be(_displayDefaultFooterItemSettings.DisplayWishlistFooterItem);
            model.DisplayApplyVendorAccountFooterItem.Should()
                .Be(_displayDefaultFooterItemSettings.DisplayApplyVendorAccountFooterItem);

            model.Topics.Should().NotBeNullOrEmpty();
            model.Topics.Count.Should().Be(4);
        }

        [Test]
        public async Task CanPrepareContactUsModel()
        {
            var model = new ContactUsModel();
            model = await _commonModelFactory.PrepareContactUsModelAsync(model, true);

            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha.Should().BeFalse();
            model.Email.Should().BeNullOrEmpty();
            model.FullName.Should().BeNullOrEmpty();

            model = await _commonModelFactory.PrepareContactUsModelAsync(model, false);
            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha.Should().BeFalse();
            model.Email.Should().Be(NopTestsDefaults.AdminEmail);
            model.FullName.Should().Be("John Smith");
        }

        [Test]
        public void PrepareContactUsModelShouldRaiseExceptionIfModelIsNull()
        {
            Assert.Throws<AggregateException>(() =>
                _commonModelFactory.PrepareContactUsModelAsync(null, true).Wait());

            Assert.Throws<AggregateException>(() =>
                _commonModelFactory.PrepareContactUsModelAsync(null, false).Wait());
        }

        [Test]
        public async Task CanPrepareContactVendorModel()
        {
            var model = new ContactVendorModel();
            model = await _commonModelFactory.PrepareContactVendorModelAsync(model, _vendor, true);
            model.Email.Should().BeNullOrEmpty();
            model.FullName.Should().BeNullOrEmpty();

            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha.Should().BeFalse();
            model.VendorId.Should().Be(_vendor.Id);
            model.VendorName.Should().Be(_vendor.Name);

            model = await _commonModelFactory.PrepareContactVendorModelAsync(model, _vendor, false);

            model.Email.Should().Be(NopTestsDefaults.AdminEmail);
            model.FullName.Should().Be("John Smith");

            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha.Should().BeFalse();
            model.VendorId.Should().Be(_vendor.Id);
            model.VendorName.Should().Be(_vendor.Name);
        }

        [Test]
        public void PrepareContactVendorModelShouldRaiseExceptionIfModelOrVendorIsNull()
        {
            Assert.Throws<AggregateException>(() =>
                _commonModelFactory.PrepareContactVendorModelAsync(null, _vendor, true).Wait());

            Assert.Throws<AggregateException>(() =>
                _commonModelFactory.PrepareContactVendorModelAsync(null, _vendor, false).Wait());

            Assert.Throws<AggregateException>(() =>
                _commonModelFactory.PrepareContactVendorModelAsync(new ContactVendorModel(), null, true).Wait());

            Assert.Throws<AggregateException>(() =>
                _commonModelFactory.PrepareContactVendorModelAsync(new ContactVendorModel(), null, false).Wait());
        }

        [Test]
        public async Task CanPrepareStoreThemeSelectorModel()
        {
            var model = await _commonModelFactory.PrepareStoreThemeSelectorModelAsync();
            model.CurrentStoreTheme.Should().NotBeNull();
            model.CurrentStoreTheme.Name.Should().Be("DefaultClean");
            model.CurrentStoreTheme.Title.Should().Be("Default clean");
            model.AvailableStoreThemes.Should().NotBeNull();
            model.AvailableStoreThemes.Count.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task CanPrepareFaviconAndAppIconsModel()
        {
            var model = await _commonModelFactory.PrepareFaviconAndAppIconsModelAsync();
            model.HeadCode.Should().Be(_commonSettings.FaviconAndAppIconsHeadCode);
        }

        [Test]
        public async Task CanPrepareRobotsTextFile()
        {
            var model = await _commonModelFactory.PrepareRobotsTextFileAsync();
            model.Should().NotBeNullOrEmpty();
            model.Trim().Split(Environment.NewLine).Length.Should().Be(74);
        }
    }
}
