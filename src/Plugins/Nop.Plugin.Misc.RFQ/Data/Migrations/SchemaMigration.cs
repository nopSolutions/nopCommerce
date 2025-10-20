using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.RFQ.Domains;

namespace Nop.Plugin.Misc.RFQ.Data.Migrations;

[NopMigration("2024/07/03 10:30:08:1687554", "Nop.Plugin.Misc.RFQ schema", MigrationProcessType.Installation)]
public class SchemaMigration : Migration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        Create.TableFor<RequestQuote>();
        Create.TableFor<RequestQuoteItem>();

        Create.TableFor<Quote>();
        Create.TableFor<QuoteItem>();
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        Delete.Table(nameof(QuoteItem));
        Delete.Table(nameof(Quote));

        Delete.Table(nameof(RequestQuoteItem));
        Delete.Table(nameof(RequestQuote));
    }
}