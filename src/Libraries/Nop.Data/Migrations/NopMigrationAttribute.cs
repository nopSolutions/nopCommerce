using System;
using System.Globalization;
using FluentMigrator;

namespace Nop.Data.Migrations
{
    public partial class NopMigrationAttribute : MigrationAttribute
    {
        private static readonly string[] _dateFormats = { "yyyy-MM-dd hh:mm:ss", "yyyy.MM.dd hh:mm:ss", "yyyy/MM/dd hh:mm:ss", "yyyy-MM-dd hh:mm:ss:fffffff", "yyyy.MM.dd hh:mm:ss:fffffff", "yyyy/MM/dd hh:mm:ss:fffffff" };

        /// <summary>
        /// Initializes a new instance of the NopMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        public NopMigrationAttribute(string dateTime) :
            base(DateTime.ParseExact(dateTime, _dateFormats, CultureInfo.InvariantCulture).Ticks, null)
        {

        }

        /// <inheritdoc />
        public NopMigrationAttribute(long version, string description) : base(version, description)
        {
        }

        /// <inheritdoc />
        public NopMigrationAttribute(long version, TransactionBehavior transactionBehavior = TransactionBehavior.Default, string description = null) : base(version, transactionBehavior, description)
        {
        }
    }
}
