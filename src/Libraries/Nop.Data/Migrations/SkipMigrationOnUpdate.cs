using System;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// Attribute to exclude migration from the list for use during the upgrade process
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SkipMigrationOnUpdateAttribute : Attribute
    {
    }
}
