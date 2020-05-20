using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.FacebookPixel.Domain;

namespace Nop.Plugin.Widgets.FacebookPixel.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/03/25 12:00:00", "Widgets.FacebookPixel base schema")]
    public class FacebookPixelSchemaMigration : AutoReversingMigration
    {
        #region Fields

        protected IMigrationManager _migrationManager;

        #endregion

        #region Ctor

        public FacebookPixelSchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            _migrationManager.BuildTable<FacebookPixelConfiguration>(Create);
        }

        #endregion
    }
}