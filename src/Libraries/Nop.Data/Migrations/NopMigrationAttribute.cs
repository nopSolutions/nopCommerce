using System;
using System.Globalization;
using FluentMigrator;
using Nop.Core;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// Attribute for a migration
    /// </summary>
    public partial class NopMigrationAttribute : MigrationAttribute
    {
        private static readonly string[] _dateFormats = { "yyyy-MM-dd HH:mm:ss", "yyyy.MM.dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss:fffffff", "yyyy.MM.dd HH:mm:ss:fffffff", "yyyy/MM/dd HH:mm:ss:fffffff" };

        private static string GetDescription(MigrationType migrationType)
        {
            return $"nopCommerce version {NopVersion.FULL_VERSION}. Update {migrationType.ToString()}";
        }
        
        private static long GetVersion(string dateTime = null)
        {
            if (!string.IsNullOrEmpty(dateTime))
                return DateTime.ParseExact(dateTime, _dateFormats, CultureInfo.InvariantCulture).Ticks;

            var date = $"{NopVersion.VERSION_STARTED_ON} 00:00:00";

            #if DEBUG
            date = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            #endif

            return GetVersion(date);
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
        /// <param name="migrationType">The migration type</param>
        public NopMigrationAttribute(MigrationType migrationType) :
            base(GetVersion() + (int)migrationType, GetDescription(migrationType))
        {
        }
    }
}
