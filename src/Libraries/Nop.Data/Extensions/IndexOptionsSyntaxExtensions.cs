using FluentMigrator.Builders.Create.Index;

namespace Nop.Data.Extensions;

/// <summary>
/// Provides extension methods for configuring index options in database migrations.
/// </summary>
/// <remarks>This class contains extension methods that add provider-specific index options to migration
/// expressions. These methods enable fluent configuration of indexes when using different database providers, such as
/// SQL Server or PostgreSQL. The extensions are intended to be used within migration scripts to enhance index
/// definitions in a provider-agnostic manner.</remarks>
public static partial class IndexOptionsSyntaxExtensions
{
    /// <summary>
    /// Specifies an additional column to be included in the index for covering queries, without being part of the index key
    /// </summary>
    /// <param name="expression">The index creation expression to extend with an included column</param>
    /// <param name="columnName">The name of the column to include in the index. Cannot be null or empty</param>
    /// <returns>The same index creation expression instance, allowing for fluent chaining of additional index configuration.</returns>
    public static ICreateIndexOnColumnSyntax Include(this ICreateIndexOnColumnSyntax expression, string columnName)
    {
        var dataSettings = DataSettingsManager.LoadSettings();

        switch (dataSettings.DataProvider)
        {
            case DataProviderType.SqlServer:
                FluentMigrator.SqlServer.SqlServerExtensions.Include(expression, columnName);
                break;
            case DataProviderType.PostgreSQL:
                FluentMigrator.Postgres.PostgresExtensions.Include(expression, columnName);
                break;
            case DataProviderType.Unknown:
            case DataProviderType.MySql:
            default:
                break;
        }

        return expression;
    }
}