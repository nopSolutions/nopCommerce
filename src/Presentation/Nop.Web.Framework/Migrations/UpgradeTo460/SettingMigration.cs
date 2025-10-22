using FluentMigrator;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
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

        var catalogSettings = this.LoadSetting<CatalogSettings>();

        //#3075
        if (!this.SettingExists(catalogSettings, settings => settings.AllowCustomersToSearchWithManufacturerName))
        {
            catalogSettings.AllowCustomersToSearchWithManufacturerName = true;
            this.SaveSetting(catalogSettings, settings => settings.AllowCustomersToSearchWithManufacturerName);
        }

        if (!this.SettingExists(catalogSettings, settings => settings.AllowCustomersToSearchWithCategoryName))
        {
            catalogSettings.AllowCustomersToSearchWithCategoryName = true;
            this.SaveSetting(catalogSettings, settings => settings.AllowCustomersToSearchWithCategoryName);
        }

        //#1933
        if (!this.SettingExists(catalogSettings, settings => settings.DisplayAllPicturesOnCatalogPages))
        {
            catalogSettings.DisplayAllPicturesOnCatalogPages = false;
            this.SaveSetting(catalogSettings, settings => settings.DisplayAllPicturesOnCatalogPages);
        }

        //#3511
        var newProductsNumber = this.GetSetting("catalogsettings.newproductsnumber");
        if (newProductsNumber is not null && int.TryParse(newProductsNumber.Value, out var newProductsPageSize))
        {
            catalogSettings.NewProductsPageSize = newProductsPageSize;
            this.SaveSetting(catalogSettings, settings => settings.NewProductsPageSize);
            this.DeleteSetting(newProductsNumber);
        }
        else if (!this.SettingExists(catalogSettings, settings => settings.NewProductsPageSize))
        {
            catalogSettings.NewProductsPageSize = 6;
            this.SaveSetting(catalogSettings, settings => settings.NewProductsPageSize);
        }

        if (!this.SettingExists(catalogSettings, settings => settings.NewProductsAllowCustomersToSelectPageSize))
        {
            catalogSettings.NewProductsAllowCustomersToSelectPageSize = false;
            this.SaveSetting(catalogSettings, settings => settings.NewProductsAllowCustomersToSelectPageSize);
        }

        if (!this.SettingExists(catalogSettings, settings => settings.NewProductsPageSizeOptions))
        {
            catalogSettings.NewProductsPageSizeOptions = "6, 3, 9";
            this.SaveSetting(catalogSettings, settings => settings.NewProductsPageSizeOptions);
        }

        //#29
        if (!this.SettingExists(catalogSettings, settings => settings.DisplayFromPrices))
        {
            catalogSettings.DisplayFromPrices = false;
            this.SaveSetting(catalogSettings, settings => settings.DisplayFromPrices);
        }

        //#6115
        if (!this.SettingExists(catalogSettings, settings => settings.ShowShortDescriptionOnCatalogPages))
        {
            catalogSettings.ShowShortDescriptionOnCatalogPages = false;
            this.SaveSetting(catalogSettings, settings => settings.ShowShortDescriptionOnCatalogPages);
        }

        var storeInformationSettings = this.LoadSetting<StoreInformationSettings>();

        //#3997
        if (!this.SettingExists(storeInformationSettings, settings => settings.InstagramLink))
        {
            storeInformationSettings.InstagramLink = "";
            this.SaveSetting(storeInformationSettings, settings => settings.InstagramLink);
        }

        var commonSettings = this.LoadSetting<CommonSettings>();

        //#5802
        if (!this.SettingExists(commonSettings, settings => settings.HeaderCustomHtml))
        {
            commonSettings.HeaderCustomHtml = "";
            this.SaveSetting(commonSettings, settings => settings.HeaderCustomHtml);
        }

        if (!this.SettingExists(commonSettings, settings => settings.FooterCustomHtml))
        {
            commonSettings.FooterCustomHtml = "";
            this.SaveSetting(commonSettings, settings => settings.FooterCustomHtml);
        }

        var orderSettings = this.LoadSetting<OrderSettings>();

        //#5604
        if (!this.SettingExists(orderSettings, settings => settings.ShowProductThumbnailInOrderDetailsPage))
        {
            orderSettings.ShowProductThumbnailInOrderDetailsPage = true;
            this.SaveSetting(orderSettings, settings => settings.ShowProductThumbnailInOrderDetailsPage);
        }

        var mediaSettings = this.LoadSetting<MediaSettings>();

        //#5604
        if (!this.SettingExists(mediaSettings, settings => settings.OrderThumbPictureSize))
        {
            mediaSettings.OrderThumbPictureSize = 80;
            this.SaveSetting(mediaSettings, settings => settings.OrderThumbPictureSize);
        }

        var adminSettings = this.LoadSetting<AdminAreaSettings>();
        if (!this.SettingExists(adminSettings, settings => settings.CheckLicense))
        {
            adminSettings.CheckLicense = true;
            this.SaveSetting(adminSettings, settings => settings.CheckLicense);
        }

        var gdprSettings = this.LoadSetting<GdprSettings>();

        //#5809
        if (!this.SettingExists(gdprSettings, settings => settings.DeleteInactiveCustomersAfterMonths))
        {
            gdprSettings.DeleteInactiveCustomersAfterMonths = 36;
            this.SaveSetting(gdprSettings, settings => settings.DeleteInactiveCustomersAfterMonths);
        }

        var captchaSettings = this.LoadSetting<CaptchaSettings>();

        //#6182
        if (!this.SettingExists(captchaSettings, settings => settings.ShowOnCheckoutPageForGuests))
        {
            captchaSettings.ShowOnCheckoutPageForGuests = false;
            this.SaveSetting(captchaSettings, settings => settings.ShowOnCheckoutPageForGuests);
        }

        //#7
        if (!this.SettingExists(mediaSettings, settings => settings.VideoIframeAllow))
        {
            mediaSettings.VideoIframeAllow = "fullscreen";
            this.SaveSetting(mediaSettings, settings => settings.VideoIframeAllow);
        }

        //#7
        if (!this.SettingExists(mediaSettings, settings => settings.VideoIframeWidth))
        {
            mediaSettings.VideoIframeWidth = 300;
            this.SaveSetting(mediaSettings, settings => settings.VideoIframeWidth);
        }

        //#7
        if (!this.SettingExists(mediaSettings, settings => settings.VideoIframeHeight))
        {
            mediaSettings.VideoIframeHeight = 150;
            this.SaveSetting(mediaSettings, settings => settings.VideoIframeHeight);
        }

        //#385
        if (!this.SettingExists(catalogSettings, settings => settings.ProductUrlStructureTypeId))
        {
            catalogSettings.ProductUrlStructureTypeId = (int)ProductUrlStructureType.Product;
            this.SaveSetting(catalogSettings, settings => settings.ProductUrlStructureTypeId);
        }

        //#5261
        var robotsTxtSettings = this.LoadSetting<RobotsTxtSettings>();

        if (!this.SettingExists(robotsTxtSettings, settings => settings.DisallowPaths))
        {
            robotsTxtSettings.DisallowPaths.AddRange(new[]
            {
                "/admin",
                "/bin/",
                "/files/",
                "/files/exportimport/",
                "/country/getstatesbycountryid",
                "/install",
                "/setproductreviewhelpfulness",
                "/*?*returnUrl="
            });

            this.SaveSetting(robotsTxtSettings, settings => settings.DisallowPaths);
        }

        if (!this.SettingExists(robotsTxtSettings, settings => settings.LocalizableDisallowPaths))
        {
            robotsTxtSettings.LocalizableDisallowPaths.AddRange(new[]
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
            });

            this.SaveSetting(robotsTxtSettings, settings => settings.LocalizableDisallowPaths);
        }

        if (!this.SettingExists(robotsTxtSettings, settings => settings.DisallowLanguages))
            this.SaveSetting(robotsTxtSettings, settings => settings.DisallowLanguages);

        if (!this.SettingExists(robotsTxtSettings, settings => settings.AdditionsRules))
            this.SaveSetting(robotsTxtSettings, settings => settings.AdditionsRules);

        if (!this.SettingExists(robotsTxtSettings, settings => settings.AllowSitemapXml))
            this.SaveSetting(robotsTxtSettings, settings => settings.AllowSitemapXml);

        //#5753
        if (!this.SettingExists(mediaSettings, settings => settings.ProductDefaultImageId))
        {
            mediaSettings.ProductDefaultImageId = 0;
            this.SaveSetting(mediaSettings, settings => settings.ProductDefaultImageId);
        }

        //#3651
        if (!this.SettingExists(orderSettings, settings => settings.AttachPdfInvoiceToOrderProcessingEmail))
        {
            orderSettings.AttachPdfInvoiceToOrderProcessingEmail = false;
            this.SaveSetting(orderSettings, settings => settings.AttachPdfInvoiceToOrderProcessingEmail);
        }

        var taxSettings = this.LoadSetting<TaxSettings>();

        //#1961
        if (!this.SettingExists(taxSettings, settings => settings.EuVatEnabledForGuests))
        {
            taxSettings.EuVatEnabledForGuests = false;
            this.SaveSetting(taxSettings, settings => settings.EuVatEnabledForGuests);
        }

        //#5570
        var sitemapXmlSettings = this.LoadSetting<SitemapXmlSettings>();

        if (!this.SettingExists(sitemapXmlSettings, settings => settings.RebuildSitemapXmlAfterHours))
        {
            sitemapXmlSettings.RebuildSitemapXmlAfterHours = 2 * 24;
            this.SaveSetting(sitemapXmlSettings, settings => settings.RebuildSitemapXmlAfterHours);
        }

        if (!this.SettingExists(sitemapXmlSettings, settings => settings.SitemapBuildOperationDelay))
        {
            sitemapXmlSettings.SitemapBuildOperationDelay = 60;
            this.SaveSetting(sitemapXmlSettings, settings => settings.SitemapBuildOperationDelay);
        }

        //#6378
        if (!this.SettingExists(mediaSettings, settings => settings.AllowSvgUploads))
        {
            mediaSettings.AllowSvgUploads = false;
            this.SaveSetting(mediaSettings, settings => settings.AllowSvgUploads);
        }

        //#5599
        var messagesSettings = this.LoadSetting<MessagesSettings>();

        if (!this.SettingExists(messagesSettings, settings => settings.UseDefaultEmailAccountForSendStoreOwnerEmails))
        {
            messagesSettings.UseDefaultEmailAccountForSendStoreOwnerEmails = false;
            this.SaveSetting(messagesSettings, settings => settings.UseDefaultEmailAccountForSendStoreOwnerEmails);
        }

        //#228
        if (!this.SettingExists(catalogSettings, settings => settings.ActiveSearchProviderSystemName))
        {
            catalogSettings.ActiveSearchProviderSystemName = string.Empty;
            this.SaveSetting(catalogSettings, settings => settings.ActiveSearchProviderSystemName);
        }

        //#43
        var metaTitleKey = $"{nameof(SeoSettings)}.DefaultTitle".ToLower();
        var metaKeywordsKey = $"{nameof(SeoSettings)}.DefaultMetaKeywords".ToLower();
        var metaDescriptionKey = $"{nameof(SeoSettings)}.DefaultMetaDescription".ToLower();
        var homepageTitleKey = $"{nameof(SeoSettings)}.HomepageTitle".ToLower();
        var homepageDescriptionKey = $"{nameof(SeoSettings)}.HomepageDescription".ToLower();

        var dataProvider = EngineContext.Current.Resolve<INopDataProvider>();

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

        dataProvider.BulkDeleteEntities<Setting>(setting => setting.Name == metaTitleKey);
        dataProvider.BulkDeleteEntities<Setting>(setting => setting.Name == metaKeywordsKey);
        dataProvider.BulkDeleteEntities<Setting>(setting => setting.Name == metaDescriptionKey);
        dataProvider.BulkDeleteEntities<Setting>(setting => setting.Name == homepageTitleKey);
        dataProvider.BulkDeleteEntities<Setting>(setting => setting.Name == homepageDescriptionKey);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}