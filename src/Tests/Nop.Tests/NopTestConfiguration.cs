namespace Nop.Tests;

public static partial class NopTestConfiguration
{
    /// <summary>
    ///  Gets the connection string for MySQL server
    /// </summary>
    public static string MySqlServerConnectionString => "";

    /// <summary>
    /// Gets the connection string for PostgreSQL server
    /// </summary>
    public static string PostgreSqlServerConnectionString => "";

    /// <summary>
    /// Gets the connection string for MS SQL server
    /// </summary>
    public static string SqlServerConnectionString => "";

    /// <summary>
    /// Gets the connection string for SQLite server
    /// </summary>
    public static string SqliteConnectionString => "Data Source=nopCommerceTest.sqlite;Mode=Memory;Cache=Shared";
}
