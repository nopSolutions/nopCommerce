using System.Collections.Generic;
using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Migrations.UpgradeTo450
{
    [NopMigration("2021-04-23 00:00:00", "4.50.0", UpdateMigrationType.Localization)]
    [SkipMigrationOnInstall]
    public class LocalizationMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            //use localizationService to add, update and delete localization resources
            localizationService.DeleteLocaleResourcesAsync(new List<string>
            {
                "Admin.Configuration.AppSettings.Hosting.UseHttpClusterHttps",
                "Admin.Configuration.AppSettings.Hosting.UseHttpClusterHttps.Hint",
                "Admin.Configuration.AppSettings.Hosting.UseHttpXForwardedProto",
                "Admin.Configuration.AppSettings.Hosting.UseHttpXForwardedProto.Hint",
                "Admin.Configuration.AppSettings.Hosting.ForwardedHttpHeader",
                "Admin.Configuration.AppSettings.Hosting.ForwardedHttpHeader.Hint"
            }).Wait();

            localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                //#5696
                ["Admin.ContentManagement.MessageTemplates.List.SearchKeywords"] = "Search keywords",
                ["Admin.ContentManagement.MessageTemplates.List.SearchKeywords.Hint"] = "Search message template(s) by specific keywords.",

				//New configurations to forward proxied headers
                ["Admin.Configuration.AppSettings.Hosting.ForwardedForHeaderName"] = "The header used to retrieve the originating client IP",
                ["Admin.Configuration.AppSettings.Hosting.ForwardedForHeaderName.Hint"] = "Specify a custom HTTP header name to determine the originating IP address (e.g., CF-Connecting-IP, X-ProxyUser-Ip).",
                ["Admin.Configuration.AppSettings.Hosting.ForwardedProtoHeaderName"] = "The header used to retrieve the value for the originating scheme",
                ["Admin.Configuration.AppSettings.Hosting.ForwardedProtoHeaderName.Hint"] = "Specify a custom HTTP header name for identifying the protocol (HTTP or HTTPS) that a client used to connect to your proxy or load balancer.",
                ["Admin.Configuration.AppSettings.Hosting.UseProxy"] = "Use proxy servers",
                ["Admin.Configuration.AppSettings.Hosting.UseProxy.Hint"] = "Enable this setting to apply forwarded headers to their matching fields on the current HTTP-request.",
                ["Admin.Configuration.AppSettings.Hosting.KnownProxies"] = "Addresses of known proxies",
                ["Admin.Configuration.AppSettings.Hosting.KnownProxies.Hint"] = "Specify a list of IP addresses (comma separated) to accept forwarded headers.",
            }).Wait();

            // rename locales
            var localesToRename = new[]
            {
                new { Name = "", NewName = "" }
            };

            var languageService = EngineContext.Current.Resolve<ILanguageService>();
            var languages = languageService.GetAllLanguagesAsync(true).Result;
            foreach (var lang in languages)
            {
                foreach (var locale in localesToRename)
                {
                    var lsr = localizationService.GetLocaleStringResourceByNameAsync(locale.Name, lang.Id, false).Result;
                    if (lsr != null)
                    {
                        lsr.ResourceName = locale.NewName;
                        localizationService.UpdateLocaleStringResourceAsync(lsr).Wait();
                    }
                }
            }
        }

        /// <summary>Collects the DOWN migration expressions</summary>
        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
