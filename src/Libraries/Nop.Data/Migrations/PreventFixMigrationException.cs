using Nop.Core;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// Represents errors that used to the prevention of fix migration in the database while development is in progress
    /// </summary>
    public class PreventFixMigrationException: NopException
    {
        public PreventFixMigrationException() : base(
            "Prevention of fix this migration in the database while development is in progress")
        {
        }
    }
}
