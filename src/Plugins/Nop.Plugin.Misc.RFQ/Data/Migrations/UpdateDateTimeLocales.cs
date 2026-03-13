using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Misc.RFQ.Data.Migrations;

[NopMigration("2026/01/13 18:41:53:1677556", "Misc.RFQ add the locale")]
public class UpdateDateTimeLocales : Migration
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        this.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Plugins.Misc.RFQ.Fields.Quote.CreatedOn.Hint"] = "The date/time (in the current customer time zone) that the quote was created.",
            ["Plugins.Misc.RFQ.Fields.RequestQuote.CreatedOn.Hint"] = "The date/time (in the current customer time zone) that the request a quote was created.",
        });
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        //nothing
    }
}