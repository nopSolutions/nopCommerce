using System.Data;
using System.Data.Common;
using System.Text;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.MySql;
using LinqToDB.SqlQuery;
using MySqlConnector;
using Nop.Core;
using Nop.Data.Mapping;

namespace Nop.Data.DataProviders;

public partial class MySqlNopDataProvider : BaseDataProvider, INopDataProvider
{
    #region Fields

    //it's quite fast hash (to cheaply distinguish between objects)
    protected const string HASH_ALGORITHM = "SHA1";

    #endregion

    #region Utilities

    /// <summary>
    /// Creates the database connection
    /// </summary>
    protected override DataConnection CreateDataConnection()
    {
        var dataContext = CreateDataConnection(LinqToDbDataProvider);

        dataContext.MappingSchema.SetDataType(typeof(Guid), new SqlDataType(DataType.NChar, typeof(Guid), 36));
        dataContext.MappingSchema.SetConvertExpression<string, Guid>(strGuid => new Guid(strGuid));

        return dataContext;
    }

    /// <summary>
    /// Gets the connection string builder
    /// </summary>
    /// <returns>The connection string builder</returns>
    protected static MySqlConnectionStringBuilder GetConnectionStringBuilder()
    {
        return new MySqlConnectionStringBuilder(GetCurrentConnectionString());
    }

    /// <summary>
    /// Gets a connection to the database for a current data provider
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Connection to a database</returns>
    protected override DbConnection GetInternalDbConnection(string connectionString)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        return new MySqlConnection(connectionString);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates the database by using the loaded connection string
    /// </summary>
    /// <param name="collation"></param>
    /// <param name="triesToConnect"></param>
    public void CreateDatabase(string collation, int triesToConnect = 10)
    {
        if (DatabaseExists())
            return;

        var builder = GetConnectionStringBuilder();

        //gets database name
        var databaseName = builder.Database;

        //now create connection string to 'master' database. It always exists.
        builder.Database = null;

        using (var connection = GetInternalDbConnection(builder.ConnectionString))
        {
            var query = $"CREATE DATABASE IF NOT EXISTS {databaseName}";
            if (!string.IsNullOrWhiteSpace(collation))
                query = $"{query} COLLATE {collation}";

            var command = connection.CreateCommand();
            command.CommandText = query;
            command.Connection.Open();

            command.ExecuteNonQuery();
        }

        //try connect
        if (triesToConnect <= 0)
            return;

        //sometimes on slow servers (hosting) there could be situations when database requires some time to be created.
        //but we have already started creation of tables and sample data.
        //as a result there is an exception thrown and the installation process cannot continue.
        //that's why we are in a cycle of "triesToConnect" times trying to connect to a database with a delay of one second.
        for (var i = 0; i <= triesToConnect; i++)
        {
            if (i == triesToConnect)
                throw new Exception("Unable to connect to the new database. Please try one more time");

            if (!DatabaseExists())
                Thread.Sleep(1000);
            else
                break;
        }
    }

    /// <summary>
    /// Checks if the specified database exists, returns true if database exists
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the returns true if the database exists.
    /// </returns>
    public async Task<bool> DatabaseExistsAsync()
    {
        try
        {
            await using var connection = GetInternalDbConnection(GetCurrentConnectionString());

            //just try to connect
            await connection.OpenAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if the specified database exists, returns true if database exists
    /// </summary>
    /// <returns>Returns true if the database exists.</returns>
    public bool DatabaseExists()
    {
        try
        {
            using var connection = CreateDbConnection();
            //just try to connect
            connection.Open();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get the current identity value
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the integer identity; null if cannot get the result
    /// </returns>
    public virtual async Task<int?> GetTableIdentAsync<TEntity>() where TEntity : BaseEntity
    {
        using var currentConnection = CreateDataConnection();
        var tableName = NopMappingSchema.GetEntityDescriptor(typeof(TEntity)).EntityName;
        var databaseName = currentConnection.Connection.Database;

        //we're using the DbConnection object until linq2db solve this issue https://github.com/linq2db/linq2db/issues/1987
        //with DataContext we could be used KeepConnectionAlive option
        await using var dbConnection = GetInternalDbConnection(GetCurrentConnectionString());

        dbConnection.StateChange += (sender, e) =>
        {
            try
            {
                if (e.CurrentState != ConnectionState.Open)
                    return;

                var connection = (IDbConnection)sender;
                using var internalCommand = connection.CreateCommand();
                internalCommand.Connection = connection;
                internalCommand.CommandText = "SET @@SESSION.information_schema_stats_expiry = 0;";
                internalCommand.ExecuteNonQuery();
            }
            //ignoring for older than 8.0 versions MySQL (#1193 Unknown system variable)
            catch (MySqlException ex) when (ex.Number == 1193)
            {
                //ignore
            }
        };

        await using var command = dbConnection.CreateCommand();
        command.Connection = dbConnection;
        command.CommandText = $"SELECT AUTO_INCREMENT FROM information_schema.TABLES WHERE TABLE_SCHEMA = '{databaseName}' AND TABLE_NAME = '{tableName}'";
        await dbConnection.OpenAsync();

        return Convert.ToInt32((await command.ExecuteScalarAsync()) ?? 1);
    }

    /// <summary>
    /// Set table identity (is supported)
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="ident">Identity value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetTableIdentAsync<TEntity>(int ident) where TEntity : BaseEntity
    {
        var currentIdent = await GetTableIdentAsync<TEntity>();
        if (!currentIdent.HasValue || ident <= currentIdent.Value)
            return;

        using var currentConnection = CreateDataConnection();
        var tableName = NopMappingSchema.GetEntityDescriptor(typeof(TEntity)).EntityName;

        await currentConnection.ExecuteAsync($"ALTER TABLE `{tableName}` AUTO_INCREMENT = {ident};");
    }

    /// <summary>
    /// Creates a backup of the database
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task BackupDatabaseAsync(string fileName)
    {
        throw new DataException("This database provider does not support backup");
    }

    /// <summary>
    /// Restores the database from a backup
    /// </summary>
    /// <param name="backupFileName">The name of the backup file</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task RestoreDatabaseAsync(string backupFileName)
    {
        throw new DataException("This database provider does not support backup");
    }

    /// <summary>
    /// Re-index database tables
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ReIndexTablesAsync()
    {
        using var currentConnection = CreateDataConnection();
        var tables = currentConnection.Query<string>($"SHOW TABLES FROM `{currentConnection.Connection.Database}`").ToList();

        if (tables.Count > 0)
            await currentConnection.ExecuteAsync($"OPTIMIZE TABLE `{string.Join("`, `", tables)}`");
    }

    /// <summary>
    /// Build the connection string
    /// </summary>
    /// <param name="nopConnectionString">Connection string info</param>
    /// <returns>Connection string</returns>
    public virtual string BuildConnectionString(INopConnectionStringInfo nopConnectionString)
    {
        ArgumentNullException.ThrowIfNull(nopConnectionString);

        if (nopConnectionString.IntegratedSecurity)
            throw new NopException("Data provider supports connection only with login and password");

        var builder = new MySqlConnectionStringBuilder
        {
            Server = nopConnectionString.ServerName,
            //Cast DatabaseName to lowercase to avoid case-sensitivity problems
            Database = nopConnectionString.DatabaseName.ToLowerInvariant(),
            AllowUserVariables = true,
            UserID = nopConnectionString.Username,
            Password = nopConnectionString.Password,
            UseXaTransactions = false
        };

        return builder.ConnectionString;
    }

    /// <summary>
    /// Gets the name of a foreign key
    /// </summary>
    /// <param name="foreignTable">Foreign key table</param>
    /// <param name="foreignColumn">Foreign key column name</param>
    /// <param name="primaryTable">Primary table</param>
    /// <param name="primaryColumn">Primary key column name</param>
    /// <returns>Name of a foreign key</returns>
    public virtual string CreateForeignKeyName(string foreignTable, string foreignColumn, string primaryTable, string primaryColumn)
    {
        //mySql support only 64 chars for constraint name
        //that is why we use hash function for create unique name
        //see details on this topic: https://dev.mysql.com/doc/refman/8.0/en/identifier-length.html
        return "FK_" + HashHelper.CreateHash(Encoding.UTF8.GetBytes($"{foreignTable}_{foreignColumn}_{primaryTable}_{primaryColumn}"), HASH_ALGORITHM);
    }

    /// <summary>
    /// Gets the name of an index
    /// </summary>
    /// <param name="targetTable">Target table name</param>
    /// <param name="targetColumn">Target column name</param>
    /// <returns>Name of an index</returns>
    public virtual string GetIndexName(string targetTable, string targetColumn)
    {
        return "IX_" + HashHelper.CreateHash(Encoding.UTF8.GetBytes($"{targetTable}_{targetColumn}"), HASH_ALGORITHM);
    }

    #endregion

    #region Properties

    /// <summary>
    /// MySql data provider
    /// </summary>
    protected override IDataProvider LinqToDbDataProvider => MySqlTools.GetDataProvider(ProviderName.MySqlConnector);

    /// <summary>
    /// Gets allowed a limit input value of the data for hashing functions, returns 0 if not limited
    /// </summary>
    public int SupportedLengthOfBinaryHash { get; } = 0;

    /// <summary>
    /// Gets a value indicating whether this data provider supports backup
    /// </summary>
    public virtual bool BackupSupported => false;

    #endregion
}