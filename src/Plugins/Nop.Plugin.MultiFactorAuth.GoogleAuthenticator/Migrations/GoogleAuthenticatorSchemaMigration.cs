using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Domains;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/07/30 12:00:00", "Nop.Plugin.MultiFactorAuth.GoogleAuthenticator schema")]
    public class GoogleAuthenticatorSchemaMigration : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public GoogleAuthenticatorSchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            _migrationManager.BuildTable<GoogleAuthenticatorRecord>(Create);
        }
    }
}
