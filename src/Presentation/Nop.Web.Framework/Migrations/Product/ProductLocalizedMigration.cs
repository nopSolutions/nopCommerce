using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.Product;

[NopUpdateMigration("2024-10-05 11:35:00", "Product Localized", UpdateMigrationType.Data)]
public class ProductLocalizedMigration : Migration
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //do not use DI, because it produces exception on the installation process
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
        var (languageId, _) = this.GetLanguageData();

        localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Admin.Catalog.Products.Fields.RequireApproval"] = "Require Approval",
            ["Admin.Catalog.Products.Fields.RequireApproval.Hint"] = "Require approval for product.",

        }, languageId);

    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
