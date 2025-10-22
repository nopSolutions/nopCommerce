﻿using FluentMigrator;
using Nop.Data;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.FacebookPixel.Domain;
using Nop.Services.Helpers;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Widgets.FacebookPixel.Data;

[NopMigration("2022-03-18 12:00:00", "Widgets.FacebookPixel 2.00. Conversions API", MigrationProcessType.Update)]
public class ConversionsApiMigration : MigrationBase
{
    #region Fields

    protected readonly ISynchronousCodeHelper _synchronousCodeHelper;

    #endregion

    #region Ctor

    public ConversionsApiMigration(ISynchronousCodeHelper synchronousCodeHelper)
    {
        _synchronousCodeHelper = synchronousCodeHelper;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var facebookPixelConfigurationTableName = NameCompatibilityManager.GetTableName(typeof(FacebookPixelConfiguration));

        //add ConversionsApiEnabled column if not exists
        if (!Schema.Table(facebookPixelConfigurationTableName).Column(nameof(FacebookPixelConfiguration.ConversionsApiEnabled)).Exists())
        {
            Alter.Table(facebookPixelConfigurationTableName)
                .AddColumn(nameof(FacebookPixelConfiguration.ConversionsApiEnabled))
                .AsBoolean().NotNullable().SetExistingRowsTo(false);
        }

        //add AccessToken column if not exists
        if (!Schema.Table(facebookPixelConfigurationTableName).Column(nameof(FacebookPixelConfiguration.AccessToken)).Exists())
        {
            Alter.Table(facebookPixelConfigurationTableName)
                .AddColumn(nameof(FacebookPixelConfiguration.AccessToken)).AsString().Nullable();
        }

        //rename Enabled column to PixelScriptEnabled
        if (Schema.Table(facebookPixelConfigurationTableName).Column("Enabled").Exists())
        {
            Rename.Column("Enabled")
                .OnTable(facebookPixelConfigurationTableName)
                .To(nameof(FacebookPixelConfiguration.PixelScriptEnabled));
        }

        //locales
        var (languageId, _) = this.GetLanguageData();

        _synchronousCodeHelper.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Plugins.Widgets.FacebookPixel.Configuration.Fields.AccessToken"] = "Access token",
            ["Plugins.Widgets.FacebookPixel.Configuration.Fields.AccessToken.Hint"] = "Enter the Facebook Conversions API access token.",
            ["Plugins.Widgets.FacebookPixel.Configuration.Fields.AccessToken.Required"] = "Access token is required",
            ["Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelScriptEnabled"] = "Pixel enabled",
            ["Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelScriptEnabled.Hint"] = "Toggle to enable/disable Facebook Pixel for this configuration.",
            ["Plugins.Widgets.FacebookPixel.Configuration.Fields.ConversionsApiEnabled"] = "Conversions API enabled",
            ["Plugins.Widgets.FacebookPixel.Configuration.Fields.ConversionsApiEnabled.Hint"] = "Toggle to enable/disable Facebook Conversions API for this configuration."
        }, languageId);

        _synchronousCodeHelper.DeleteLocaleResources(new List<string>
        {
            "Plugins.Widgets.FacebookPixel.Configuration.Fields.Enabled",
            "Plugins.Widgets.FacebookPixel.Configuration.Fields.Enabled.Hint"
        });
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        //nothing
    }

    #endregion
}