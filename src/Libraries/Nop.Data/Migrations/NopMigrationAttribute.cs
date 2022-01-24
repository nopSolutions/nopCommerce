using System;
using System.Globalization;
using FluentMigrator;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// Attribute for a migration
    /// </summary>
    public partial class NopMigrationAttribute : MigrationAttribute
    {
        #region Utils

        protected static long GetVersion(string dateTime)
        {
            return DateTime.ParseExact(dateTime, NopMigrationDefaults.DateFormats, CultureInfo.InvariantCulture).Ticks;
        }

        protected static long GetVersion(string dateTime, UpdateMigrationType migrationType)
        {
            return GetVersion(dateTime) + (int)migrationType;
        }

        protected static string GetDescription(string nopVersion, UpdateMigrationType migrationType)
        {
            return string.Format(NopMigrationDefaults.UpdateMigrationDescription, nopVersion, migrationType.ToString());
        }

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the NopMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        /// <param name="targetMigrationProcess">The target migration process</param>
        public NopMigrationAttribute(string dateTime, MigrationProcessType targetMigrationProcess = MigrationProcessType.NoMatter) :
            base(GetVersion(dateTime), null)
        {
            TargetMigrationProcess = targetMigrationProcess;
        }

        /// <summary>
        /// Initializes a new instance of the NopMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        /// <param name="description">The migration description</param>
        /// <param name="targetMigrationProcess">The target migration process</param>
        public NopMigrationAttribute(string dateTime, string description, MigrationProcessType targetMigrationProcess = MigrationProcessType.NoMatter) :
            base(GetVersion(dateTime), description)
        {
            TargetMigrationProcess = targetMigrationProcess;
        }

        /// <summary>
        /// Initializes a new instance of the NopMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        /// <param name="nopVersion">nopCommerce full version</param>
        /// <param name="migrationType">The migration type</param>
        /// <param name="targetMigrationProcess">The target migration process</param>
        public NopMigrationAttribute(string dateTime, string nopVersion, UpdateMigrationType migrationType, MigrationProcessType targetMigrationProcess = MigrationProcessType.NoMatter) :
            base(GetVersion(dateTime, migrationType), GetDescription(nopVersion, migrationType))
        {
            TargetMigrationProcess = targetMigrationProcess;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Target migration process
        /// </summary>
        public MigrationProcessType TargetMigrationProcess { get; set; }

        #endregion
    }
}
