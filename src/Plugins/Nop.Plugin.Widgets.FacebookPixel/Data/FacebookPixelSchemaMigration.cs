using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.FacebookPixel.Domain;

namespace Nop.Plugin.Widgets.FacebookPixel.Data;

[NopMigration("2020/03/25 12:00:00", "Widgets.FacebookPixel base schema", MigrationProcessType.Installation)]
public class FacebookPixelSchemaMigration : AutoReversingMigration
{

    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        Create.TableFor<FacebookPixelConfiguration>();
    }

    #endregion
}