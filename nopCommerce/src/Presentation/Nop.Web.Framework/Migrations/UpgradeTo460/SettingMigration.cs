using FluentMigrator;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Helpers;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo460;

[NopUpdateMigration("2023-07-26 14:00:00", "4.60", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //do not use DI, because it produces exception on the installation process
        var syncCodeHelper = EngineContext.Current.Resolve<ISyncCodeHelper>();

        //#3075
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.AllowCustomersToSearchWithManufacturerName, true);
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.AllowCustomersToSearchWithCategoryName, true);

        //#1933
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.DisplayAllPicturesOnCatalogPages, false);

        //#3511
        this.SetSettingIfNotExists<CatalogSettings, int>(settings => settings.NewProductsPageSize, setting =>
        {
            var newProductsNumber = this.GetSettingByKey<string>("catalogsettings.newproductsnumber");
            if (newProductsNumber is not null && int.TryParse(newProductsNumber, out var newProductsPageSize))
            {
                setting.NewProductsPageSize = newProductsPageSize;
                this.DeleteSettingsByNames(["catalogsettings.newproductsnumber"]);
            }
            else
            {
                setting.NewProductsPageSize = 6;
            }
        });

        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.NewProductsAllowCustomersToSelectPageSize, false);
        this.SetSettingIfNotExists<CatalogSettings, string>(settings => settings.NewProductsPageSizeOptions, "6, 3, 9");

        //#29
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.DisplayFromPrices, false);

        //#6115
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.ShowShortDescriptionOnCatalogPages, false);

        //#3997
        this.SetSettingIfNotExists<StoreInformationSettings, string>(settings => settings.InstagramLink, string.Empty);

        //#5802
        this.SetSettingIfNotExists<CommonSettings, string>(settings => settings.HeaderCustomHtml, string.Empty);
        this.SetSettingIfNotExists<CommonSettings, string>(settings => settings.FooterCustomHtml, string.Empty);

        //#5604
        this.SetSettingIfNotExists<OrderSettings, bool>(settings => settings.ShowProductThumbnailInOrderDetailsPage, true);

        //#5604
        this.SetSettingIfNotExists<MediaSettings, int>(settings => settings.OrderThumbPictureSize, 80);
        this.SetSettingIfNotExists<AdminAreaSettings, bool>(settings => settings.CheckLicense, true);

        //#5809
        this.SetSettingIfNotExists<GdprSettings, int>(settings => settings.DeleteInactiveCustomersAfterMonths, 36);

        //#6182
        this.SetSettingIfNotExists<CaptchaSettings, bool>(settings => settings.ShowOnCheckoutPageForGuests, false);

        //#7
        this.SetSettingIfNotExists<MediaSettings, string>(settings => settings.VideoIframeAllow, "fullscreen");
        this.SetSettingIfNotExists<MediaSettings, int>(settings => settings.VideoIframeWidth, 300);
        this.SetSettingIfNotExists<MediaSettings, int>(settings => settings.VideoIframeHeight, 150);

        //#385
        this.SetSettingIfNotExists<CatalogSettings, int>(settings => settings.ProductUrlStructureTypeId, (int)ProductUrlStructureType.Product);

        //#5261
        this.SetSettingIfNotExists<RobotsTxtSettings, List<string>>(settings => settings.DisallowPaths, setting => setting.DisallowPaths.AddRange(new[]
        {
            "/admin",
            "/bin/",
            "/files/",
            "/files/exportimport/",
            "/country/getstatesbycountryid",
            "/install",
            "/setproductreviewhelpfulness",
            "/*?*returnUrl="
        }));
        this.SetSettingIfNotExists<RobotsTxtSettings, List<string>>(settings => settings.LocalizableDisallowPaths, setting => setting.LocalizableDisallowPaths.AddRange(new[]
            {
                "/addproducttocart/catalog/",
                "/addproducttocart/details/",
                "/backinstocksubscriptions/manage",
                "/boards/forumsubscriptions",
                "/boards/forumwatch",
                "/boards/postedit",
                "/boards/postdelete",
                "/boards/postcreate",
                "/boards/topicedit",
                "/boards/topicdelete",
                "/boards/topiccreate",
                "/boards/topicmove",
                "/boards/topicwatch",
                "/cart$",
                "/changecurrency",
                "/changelanguage",
                "/changetaxtype",
                "/checkout",
                "/checkout/billingaddress",
                "/checkout/completed",
                "/checkout/confirm",
                "/checkout/shippingaddress",
                "/checkout/shippingmethod",
                "/checkout/paymentinfo",
                "/checkout/paymentmethod",
                "/clearcomparelist",
                "/compareproducts",
                "/compareproducts/add/*",
                "/customer/avatar",
                "/customer/activation",
                "/customer/addresses",
                "/customer/changepassword",
                "/customer/checkusernameavailability",
                "/customer/downloadableproducts",
                "/customer/info",
                "/customer/productreviews",
                "/deletepm",
                "/emailwishlist",
                "/eucookielawaccept",
                "/inboxupdate",
                "/newsletter/subscriptionactivation",
                "/onepagecheckout",
                "/order/history",
                "/orderdetails",
                "/passwordrecovery/confirm",
                "/poll/vote",
                "/privatemessages",
                "/recentlyviewedproducts",
                "/returnrequest",
                "/returnrequest/history",
                "/rewardpoints/history",
                "/search?",
                "/sendpm",
                "/sentupdate",
                "/shoppingcart/*",
                "/storeclosed",
                "/subscribenewsletter",
                "/topic/authenticate",
                "/viewpm",
                "/uploadfilecheckoutattribute",
                "/uploadfileproductattribute",
                "/uploadfilereturnrequest",
                "/wishlist"
            }));
        this.SetSettingIfNotExists<RobotsTxtSettings, List<int>>(settings => settings.DisallowLanguages);
        this.SetSettingIfNotExists<RobotsTxtSettings, List<string>>(settings => settings.AdditionsRules);
        this.SetSettingIfNotExists<RobotsTxtSettings, bool>(settings => settings.AllowSitemapXml);

        //#5753
        this.SetSettingIfNotExists<MediaSettings, int>(settings => settings.ProductDefaultImageId, 0);

        //#3651
        this.SetSettingIfNotExists<OrderSettings, bool>(settings => settings.AttachPdfInvoiceToOrderProcessingEmail, false);

        //#1961
        this.SetSettingIfNotExists<TaxSettings, bool>(settings => settings.EuVatEnabledForGuests, false);

        //#5570
        this.SetSettingIfNotExists<SitemapXmlSettings, int>(settings => settings.RebuildSitemapXmlAfterHours, 2 * 24);
        this.SetSettingIfNotExists<SitemapXmlSettings, int>(settings => settings.SitemapBuildOperationDelay, 60);

        //#6378
        this.SetSettingIfNotExists<MediaSettings, bool>(settings => settings.AllowSvgUploads, false);

        //#5599
        this.SetSettingIfNotExists<MessagesSettings, bool>(settings => settings.UseDefaultEmailAccountForSendStoreOwnerEmails, false);

        //#228
        this.SetSettingIfNotExists<CatalogSettings, string>(settings => settings.ActiveSearchProviderSystemName, string.Empty);

        //#43
        var metaTitleKey = $"{nameof(SeoSettings)}.DefaultTitle".ToLower();
        var metaKeywordsKey = $"{nameof(SeoSettings)}.DefaultMetaKeywords".ToLower();
        var metaDescriptionKey = $"{nameof(SeoSettings)}.DefaultMetaDescription".ToLower();
        var homepageTitleKey = $"{nameof(SeoSettings)}.HomepageTitle".ToLower();
        var homepageDescriptionKey = $"{nameof(SeoSettings)}.HomepageDescription".ToLower();

        foreach (var store in syncCodeHelper.GetAllEntities<Store>(query => query.OrderBy(s => s.DisplayOrder).ThenBy(s => s.Id), _ => default, includeDeleted: false))
        {
            var metaTitle = this.GetSettingByKey<string>(metaTitleKey, storeId: store.Id) ?? this.GetSettingByKey<string>(metaTitleKey);
            var metaKeywords = this.GetSettingByKey<string>(metaKeywordsKey, storeId: store.Id) ?? this.GetSettingByKey<string>(metaKeywordsKey);
            var metaDescription = this.GetSettingByKey<string>(metaDescriptionKey, storeId: store.Id) ?? this.GetSettingByKey<string>(metaDescriptionKey);
            var homepageTitle = this.GetSettingByKey<string>(homepageTitleKey, storeId: store.Id) ?? this.GetSettingByKey<string>(homepageTitleKey);
            var homepageDescription = this.GetSettingByKey<string>(homepageDescriptionKey, storeId: store.Id) ?? this.GetSettingByKey<string>(homepageDescriptionKey);

            if (metaTitle != null)
                store.DefaultTitle = metaTitle;

            if (metaKeywords != null)
                store.DefaultMetaKeywords = metaKeywords;

            if (metaDescription != null)
                store.DefaultMetaDescription = metaDescription;

            if (homepageTitle != null)
                store.HomepageTitle = homepageTitle;

            if (homepageDescription != null)
                store.HomepageDescription = homepageDescription;

            syncCodeHelper.UpdateEntity(store);
        }

        this.DeleteSettingsByNames([metaTitleKey, metaKeywordsKey, metaDescriptionKey, homepageTitleKey, homepageDescriptionKey]);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}