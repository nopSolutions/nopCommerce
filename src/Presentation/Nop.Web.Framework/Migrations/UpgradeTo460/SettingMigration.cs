using FluentMigrator;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Web.Framework.Migrations.UpgradeTo460
{
    [NopMigration("2022-02-08 00:00:00", "4.60.0", UpdateMigrationType.Settings, MigrationProcessType.Update)]
    public class SettingMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            var catalogSettings = settingService.LoadSettingAsync<CatalogSettings>().Result;

            //#3075
            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.AllowCustomersToSearchWithManufacturerName).Result)
            {
                catalogSettings.AllowCustomersToSearchWithManufacturerName = true;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.AllowCustomersToSearchWithManufacturerName).Wait();
            }

            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.AllowCustomersToSearchWithCategoryName).Result)
            {
                catalogSettings.AllowCustomersToSearchWithCategoryName = true;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.AllowCustomersToSearchWithCategoryName).Wait();
            }

            //#1933
            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.DisplayAllPicturesOnCatalogPages).Result)
            {
                catalogSettings.DisplayAllPicturesOnCatalogPages = false;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.DisplayAllPicturesOnCatalogPages).Wait();
            }
            
            //#3511
            var newProductsNumber = settingService.GetSettingAsync("catalogsettings.newproductsnumber").Result;
            if (newProductsNumber is not null && int.TryParse(newProductsNumber.Value, out var newProductsPageSize))
            {
                catalogSettings.NewProductsPageSize = newProductsPageSize;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.NewProductsPageSize).Wait();
                settingService.DeleteSettingAsync(newProductsNumber).Wait();
            }
            else if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.NewProductsPageSize).Result)
            {
                catalogSettings.NewProductsPageSize = 6;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.NewProductsPageSize).Wait();
            }

            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.NewProductsAllowCustomersToSelectPageSize).Result)
            {
                catalogSettings.NewProductsAllowCustomersToSelectPageSize = false;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.NewProductsAllowCustomersToSelectPageSize).Wait();
            }

            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.NewProductsPageSizeOptions).Result)
            {
                catalogSettings.NewProductsPageSizeOptions = "6, 3, 9";
                settingService.SaveSettingAsync(catalogSettings, settings => settings.NewProductsPageSizeOptions).Wait();
            }

            //#29
            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.DisplayFromPrices).Result)
            {
                catalogSettings.DisplayFromPrices = false;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.DisplayFromPrices).Wait();
            }

            //#6115
            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.ShowShortDescriptionOnCatalogPages).Result)
            {
                catalogSettings.ShowShortDescriptionOnCatalogPages = false;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.ShowShortDescriptionOnCatalogPages).Wait();
            }

            var storeInformationSettings = settingService.LoadSettingAsync<StoreInformationSettings>().Result;

            //#3997
            if (!settingService.SettingExistsAsync(storeInformationSettings, settings => settings.InstagramLink).Result)
            {
                storeInformationSettings.InstagramLink = "";
                settingService.SaveSettingAsync(storeInformationSettings, settings => settings.InstagramLink).Wait();
            }

            var commonSettings = settingService.LoadSettingAsync<CommonSettings>().Result;

            //#5802
            if (!settingService.SettingExistsAsync(commonSettings, settings => settings.HeaderCustomHtml).Result)
            {
                commonSettings.HeaderCustomHtml = "";
                settingService.SaveSettingAsync(commonSettings, settings => settings.HeaderCustomHtml).Wait();
            }

            if (!settingService.SettingExistsAsync(commonSettings, settings => settings.FooterCustomHtml).Result)
            {
                commonSettings.FooterCustomHtml = "";
                settingService.SaveSettingAsync(commonSettings, settings => settings.FooterCustomHtml).Wait();
            }

            var orderSettings = settingService.LoadSettingAsync<OrderSettings>().Result;

            //#5604
            if (!settingService.SettingExistsAsync(orderSettings, settings => settings.ShowProductThumbnailInOrderDetailsPage).Result)
            {
                orderSettings.ShowProductThumbnailInOrderDetailsPage = true;
                settingService.SaveSettingAsync(orderSettings, settings => settings.ShowProductThumbnailInOrderDetailsPage).Wait();
            }

            var mediaSettings = settingService.LoadSettingAsync<MediaSettings>().Result;

            //#5604
            if (!settingService.SettingExistsAsync(mediaSettings, settings => settings.OrderThumbPictureSize).Result)
            {
                mediaSettings.OrderThumbPictureSize = 80;
                settingService.SaveSettingAsync(mediaSettings, settings => settings.OrderThumbPictureSize).Wait();
            }

            var gdprSettings = settingService.LoadSettingAsync<GdprSettings>().Result;

            //#5809
            if (!settingService.SettingExistsAsync(gdprSettings, settings => settings.DeleteInactiveCustomersAfterMonths).Result)
            {
                gdprSettings.DeleteInactiveCustomersAfterMonths = 36;
                settingService.SaveSettingAsync(gdprSettings, settings => settings.DeleteInactiveCustomersAfterMonths).Wait();
            }

            var captchaSettings = settingService.LoadSettingAsync<CaptchaSettings>().Result;

            //#6182
            if (!settingService.SettingExistsAsync(captchaSettings, settings => settings.ShowOnCheckoutPageForGuests).Result)
            {
                captchaSettings.ShowOnCheckoutPageForGuests = false;
                settingService.SaveSettingAsync(captchaSettings, settings => settings.ShowOnCheckoutPageForGuests).Wait();
            }
            
            //#7
            if (!settingService.SettingExistsAsync(mediaSettings, settings => settings.VideoIframeAllow).Result)
            {
                mediaSettings.VideoIframeAllow = "fullscreen";
                settingService.SaveSettingAsync(mediaSettings, settings => settings.VideoIframeAllow).Wait();
            }

            //#7
            if (!settingService.SettingExistsAsync(mediaSettings, settings => settings.VideoIframeWidth).Result)
            {
                mediaSettings.VideoIframeWidth = 300;
                settingService.SaveSettingAsync(mediaSettings, settings => settings.VideoIframeWidth).Wait();
            }

            //#7
            if (!settingService.SettingExistsAsync(mediaSettings, settings => settings.VideoIframeHeight).Result)
            {
                mediaSettings.VideoIframeHeight = 150;
                settingService.SaveSettingAsync(mediaSettings, settings => settings.VideoIframeHeight).Wait();
            }

            //#385
            if (!settingService.SettingExistsAsync(catalogSettings, settings => settings.ProductUrlStructureTypeId).Result)
            {
                catalogSettings.ProductUrlStructureTypeId = (int)ProductUrlStructureType.Product;
                settingService.SaveSettingAsync(catalogSettings, settings => settings.ProductUrlStructureTypeId).Wait();
            }

            //#5261
            var robotsTxtSettings = settingService.LoadSettingAsync<RobotsTxtSettings>().Result;

            if (!settingService.SettingExistsAsync(robotsTxtSettings, settings => settings.DisallowPaths).Result)
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

                settingService.SaveSettingAsync(robotsTxtSettings, settings => settings.DisallowPaths).Wait();
            }

            if (!settingService.SettingExistsAsync(robotsTxtSettings, settings => settings.LocalizableDisallowPaths).Result)
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

                settingService.SaveSettingAsync(robotsTxtSettings, settings => settings.LocalizableDisallowPaths).Wait();
            }

            if (!settingService.SettingExistsAsync(robotsTxtSettings, settings => settings.DisallowLanguages).Result) 
                settingService.SaveSettingAsync(robotsTxtSettings, settings => settings.DisallowLanguages).Wait();
            
            if (!settingService.SettingExistsAsync(robotsTxtSettings, settings => settings.AdditionsRules).Result)
                settingService.SaveSettingAsync(robotsTxtSettings, settings => settings.AdditionsRules).Wait();

            if (!settingService.SettingExistsAsync(robotsTxtSettings, settings => settings.AllowSitemapXml).Result)
                settingService.SaveSettingAsync(robotsTxtSettings, settings => settings.AllowSitemapXml).Wait();
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}