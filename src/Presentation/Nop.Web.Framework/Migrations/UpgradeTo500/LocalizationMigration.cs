using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo500;

[NopUpdateMigration("2025-10-27 00:00:00", "5.00", UpdateMigrationType.Localization)]
public class LocalizationMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //do not use DI, because it produces exception on the installation process
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        var (languageId, languages) = this.GetLanguageData();

        #region Delete locales

        localizationService.DeleteLocaleResources(new List<string>
        {
            
        });

        #endregion

        #region Rename locales

        this.RenameLocales(new Dictionary<string, string>
        {
            
        }, languages, localizationService);
        
        #endregion

        #region Add or update locales

        localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            //#7898
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.LogRequests"] = "Log AI requests",
            ["Admin.Configuration.Settings.Catalog.ArtificialIntelligence.LogRequests.Hint"] = "Check to enable logging of all requests to AI services.",

            //7400
            ["Admin.Catalog.Manufacturers.Gspr"] = "GSPR",
            ["Admin.Configuration.Settings.Catalog.BlockTitle.Gsps"] = "GSPS",
            ["Admin.Configuration.Settings.Gspr.Enabled"] = "Enabled",
            ["Admin.Configuration.Settings.Gspr.Enabled.Hint"] = "Check to display GSPR information in public store. The General Product Safety Regulation (GPSR) is a European Union (EU) law that came into effect in December 2024. The GPSR regulates the advertising and sale of consumer products in the European Union",
            ["Admin.Catalog.Manufacturers.Gspr.Information"] = "<p>The General Product Safety Regulation (GPSR) is a European Union (EU) law that came into effect in December 2024. The GPSR regulates the advertising and sale of consumer products in the European Union.</p><p>If you are in the EU, you have to fill in the manufacturer’s name, physical address and e-mail or other electronic contact information.</p><p>If the manufacturer is not located in the European Union, the EU Responsible Person and their contact details. The EU Responsible Person is a person or entity located in the European Union that the seller designates to act as a point of contact with the EU authorities.</p>",
            ["Admin.Catalog.Manufacturers.Fields.PhysicalAddress"] = "Manufacturer physical address",
            ["Admin.Catalog.Manufacturers.Fields.PhysicalAddress.Hint"] = "The physical address of the manufacturer.",
            ["Admin.Catalog.Manufacturers.Fields.ElectronicAddress"] = "Manufacturer electronic address",
            ["Admin.Catalog.Manufacturers.Fields.PhysicalAddress.Hint"] = "The e-mail or other electronic contact information.",
            ["Admin.Catalog.Manufacturers.Fields.Admin.Catalog.Manufacturers.Fields.ResponsiblePerson"] = "Responsible person name",
            ["Admin.Catalog.Manufacturers.Fields.Admin.Catalog.Manufacturers.Fields.ResponsiblePerson.Hint"] = "The name of the manufacturer's responsible person.",
            ["Admin.Catalog.Manufacturers.Fields.ResponsiblePerson"] = "Responsible person name",
            ["Admin.Catalog.Manufacturers.Fields.ResponsiblePerson.Hint"] = "The name of the manufacturer's responsible person",
            ["Admin.Catalog.Manufacturers.Fields.ResponsiblePersonPhysicalAddress"] = "Responsible person physical address",
            ["Admin.Catalog.Manufacturers.Fields.ResponsiblePersonPhysicalAddress.Hint"] = "The physical address of the manufacturer's responsible person.",
            ["Admin.Catalog.Manufacturers.Fields.ResponsiblePersonElectronicAddress"] = "Responsible person electronic address",
            ["Admin.Catalog.Manufacturers.Fields.ResponsiblePersonElectronicAddress.Hint"] = "The e-mail or other electronic contact information of the manufacturer's responsible person.",
        }, languageId);

        #endregion
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
