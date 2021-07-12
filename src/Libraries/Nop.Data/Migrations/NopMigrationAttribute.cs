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
        private static long GetVersion(string dateTime)
        {
            return DateTime.ParseExact(dateTime, NopMigrationDefaults.DateFormats, CultureInfo.InvariantCulture).Ticks;
        }

        private static long GetVersion(string dateTime, UpdateMigrationType migrationType)
        {
            return GetVersion(dateTime) + (int)migrationType;
        }
        
        private static string GetDescription(string nopVersion, UpdateMigrationType migrationType)
        {
            return string.Format(NopMigrationDefaults.UpdateMigrationDescription, nopVersion, migrationType.ToString());
        }

        /// <summary>
        /// Initializes a new instance of the NopMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        public NopMigrationAttribute(string dateTime) :
            base(GetVersion(dateTime), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NopMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        /// <param name="description">The migration description</param>
        public NopMigrationAttribute(string dateTime, string description) :
            base(GetVersion(dateTime), description)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NopMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        /// <param name="nopVersion">nopCommerce full version</param>
        /// <param name="migrationType">The migration type</param>
        public NopMigrationAttribute(string dateTime, string nopVersion, UpdateMigrationType migrationType) :
            base(GetVersion(dateTime, migrationType), GetDescription(nopVersion, migrationType))
        {
        }
    }
}
