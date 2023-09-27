namespace Nop.Data.Migrations
{
    /// <summary>
    /// Attribute for schema migration
    /// </summary>
    ///<remarks>
    /// This migration will apply right after the migration runner will become available.
    /// Do not us dependency injection in migrations that are marked by this attribute,
    /// because IoC container not ready yet.
    ///</remarks>
    public partial class NopSchemaMigrationAttribute : NopMigrationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the NopSchemaMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        /// <param name="description">The migration description</param>
        /// <param name="targetMigrationProcess">The target migration process</param>
        public NopSchemaMigrationAttribute(string dateTime, string description,
            MigrationProcessType targetMigrationProcess = MigrationProcessType.Update) :
            base(dateTime, description, targetMigrationProcess)
        {
            IsSchemaMigration = true;
        }
    }
}