using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Payments.MercadoPagoPlugin.Domains;

namespace Nop.Plugin.Payments.MercadoPagoPlugin.Migrations;
[NopMigration("17/02/2025 0:41:30", "Nop.Plugin.Payments.MercadoPagoPlugin schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        Create.TableFor<CustomTable>();
    }
}