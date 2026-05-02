using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Domains;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Migrations;

[NopMigration("2020/07/30 12:00:00", "Nop.Plugin.MultiFactorAuth.GoogleAuthenticator schema", MigrationProcessType.Installation)]
public class GoogleAuthenticatorSchemaMigration : Migration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        this.CreateTableIfNotExists<GoogleAuthenticatorRecord>();
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        this.DeleteTableIfExists<GoogleAuthenticatorRecord>();
    }
}