using System.Data.Common;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.SqlServer;
using Microsoft.Data.SqlClient;
using Nop.Core;
using Nop.Data.Mapping;

namespace Nop.Data.DataProviders;

/// <summary>
/// Represents the MS SQL Server data provider
/// </summary>
public partial class MsSqlNopDataProvider : BaseDataProvider, INopDataProvider
{
    #region Utilities

    /// <summary>
    /// Gets the connection string builder
    /// </summary>
    /// <returns>The connection string builder</returns>
    protected static SqlConnectionStringBuilder GetConnectionStringBuilder()
    {
        var connectionString = DataSettingsManager.LoadSettings().ConnectionString;

        return new SqlConnectionStringBuilder(connectionString);
    }

    /// <summary>
    /// Gets a connection to the database for a current data provider
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Connection to a database</returns>
    protected override DbConnection GetInternalDbConnection(string connectionString)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        return new SqlConnection(connectionString);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create the database
    /// </summary>
    /// <param name="collation">Collation</param>
    /// <param name="triesToConnect">Count of tries to connect to the database after creating; set 0 if no need to connect after creating</param>
    public void CreateDatabase(string collation, int triesToConnect = 10)
    {
        if (DatabaseExists())
            return;

        var builder = GetConnectionStringBuilder();

        //gets database name
        var databaseName = builder.InitialCatalog;

        //now create connection string to 'master' dabatase. It always exists.
        builder.InitialCatalog = "master";

        using (var connection = GetInternalDbConnection(builder.ConnectionString))
        {
            var query = $"CREATE DATABASE [{databaseName}]";
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
            using var connection = GetInternalDbConnection(GetCurrentConnectionString());
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
    /// Returns queryable source for specified mapping class for current connection,
    /// mapped to database table or view.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Queryable source</returns>
    public override IQueryable<TEntity> GetTable<TEntity>()
    {
        var table = (ITable<TEntity>)base.GetTable<TEntity>();

        return DataSettingsManager.UseNoLock() ? table.With("NOLOCK") : table;
    }

    /// <summary>
    /// Get the current identity value
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the integer identity; null if cannot get the result
    /// </returns>
    public virtual Task<int?> GetTableIdentAsync<TEntity>() where TEntity : BaseEntity
    {
        using var currentConnection = CreateDataConnection();
        var tableName = NopMappingSchema.GetEntityDescriptor(typeof(TEntity)).EntityName;

        var result = currentConnection.Query<decimal?>($"SELECT IDENT_CURRENT('[{tableName}]') as Value")
            .FirstOrDefault();

        return Task.FromResult<int?>(result.HasValue ? Convert.ToInt32(result) : 1);
    }

    /// <summary>
    /// Set table identity (is supported)
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="ident">Identity value</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetTableIdentAsync<TEntity>(int ident) where TEntity : BaseEntity
    {
        using var currentConnection = CreateDataConnection();
        var currentIdent = await GetTableIdentAsync<TEntity>();
        if (!currentIdent.HasValue || ident <= currentIdent.Value)
            return;

        var tableName = NopMappingSchema.GetEntityDescriptor(typeof(TEntity)).EntityName;

        await currentConnection.ExecuteAsync($"DBCC CHECKIDENT([{tableName}], RESEED, {ident})");
    }

    /// <summary>
    /// Creates a backup of the database
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task BackupDatabaseAsync(string fileName)
    {
        using var currentConnection = CreateDataConnection();
        var commandText = $"BACKUP DATABASE [{currentConnection.Connection.Database}] TO DISK = '{fileName}' WITH FORMAT";
        await currentConnection.ExecuteAsync(commandText);
    }

    /// <summary>
    /// Restores the database from a backup
    /// </summary>
    /// <param name="backupFileName">The name of the backup file</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task RestoreDatabaseAsync(string backupFileName)
    {
        using var currentConnection = CreateDataConnection();
        var commandText = string.Format(
            "DECLARE @ErrorMessage NVARCHAR(4000)\n" +
            "ALTER DATABASE [{0}] SET OFFLINE WITH ROLLBACK IMMEDIATE\n" +
            "BEGIN TRY\n" +
            "RESTORE DATABASE [{0}] FROM DISK = '{1}' WITH REPLACE\n" +
            "END TRY\n" +
            "BEGIN CATCH\n" +
            "SET @ErrorMessage = ERROR_MESSAGE()\n" +
            "END CATCH\n" +
            "ALTER DATABASE [{0}] SET MULTI_USER WITH ROLLBACK IMMEDIATE\n" +
            "IF (@ErrorMessage is not NULL)\n" +
            "BEGIN\n" +
            "RAISERROR (@ErrorMessage, 16, 1)\n" +
            "END",
            currentConnection.Connection.Database,
            backupFileName);

        await currentConnection.ExecuteAsync(commandText);
    }

    /// <summary>
    /// Re-index database tables
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ReIndexTablesAsync()
    {
        using var currentConnection = CreateDataConnection();
        var commandText = $@"
                    DECLARE @TableName sysname 
                    DECLARE cur_reindex CURSOR FOR
                    SELECT table_name
                    FROM [{currentConnection.Connection.Database}].INFORMATION_SCHEMA.TABLES
                    WHERE table_type = 'base table'
                    OPEN cur_reindex
                    FETCH NEXT FROM cur_reindex INTO @TableName
                    WHILE @@FETCH_STATUS = 0
                        BEGIN
                            exec('ALTER INDEX ALL ON [' + @TableName + '] REBUILD')
                            FETCH NEXT FROM cur_reindex INTO @TableName
                        END
                    CLOSE cur_reindex
                    DEALLOCATE cur_reindex";

        await currentConnection.ExecuteAsync(commandText);
    }

    /// <summary>
    /// Build the connection string
    /// </summary>
    /// <param name="nopConnectionString">Connection string info</param>
    /// <returns>Connection string</returns>
    public virtual string BuildConnectionString(INopConnectionStringInfo nopConnectionString)
    {
        ArgumentNullException.ThrowIfNull(nopConnectionString);

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = nopConnectionString.ServerName,
            InitialCatalog = nopConnectionString.DatabaseName,
            PersistSecurityInfo = false,
            IntegratedSecurity = nopConnectionString.IntegratedSecurity,
            TrustServerCertificate = true
        };

        if (!nopConnectionString.IntegratedSecurity)
        {
            builder.UserID = nopConnectionString.Username;
            builder.Password = nopConnectionString.Password;
        }

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
        return $"FK_{foreignTable}_{foreignColumn}_{primaryTable}_{primaryColumn}";
    }

    /// <summary>
    /// Gets the name of an index
    /// </summary>
    /// <param name="targetTable">Target table name</param>
    /// <param name="targetColumn">Target column name</param>
    /// <returns>Name of an index</returns>
    public virtual string GetIndexName(string targetTable, string targetColumn)
    {
        return $"IX_{targetTable}_{targetColumn}";
    }

    /// <summary>
    /// Updates records in table, using values from entity parameter.
    /// Records to update are identified by match on primary key value from obj value.
    /// </summary>
    /// <param name="entities">Entities with data to update</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UpdateEntitiesAsync<TEntity>(IEnumerable<TEntity> entities)
    {
        using var dataContext = CreateDataConnection();
        await dataContext.GetTable<TEntity>()
            .Merge()
            .Using(entities)
            .OnTargetKey()
            .UpdateWhenMatched()
            .MergeAsync();
    }

    /// <summary>
    /// Updates records in table, using values from entity parameter.
    /// Records to update are identified by match on primary key value from obj value.
    /// </summary>
    /// <param name="entities">Entities with data to update</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public override void UpdateEntities<TEntity>(IEnumerable<TEntity> entities)
    {
        using var dataContext = CreateDataConnection();
        dataContext.GetTable<TEntity>()
            .Merge()
            .Using(entities)
            .OnTargetKey()
            .UpdateWhenMatched()
            .Merge();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Sql server data provider
    /// </summary>
    protected override IDataProvider LinqToDbDataProvider => SqlServerTools.GetDataProvider(SqlServerVersion.v2012, SqlServerProvider.MicrosoftDataSqlClient);

    /// <summary>
    /// Gets allowed a limit input value of the data for hashing functions, returns 0 if not limited
    /// </summary>
    public int SupportedLengthOfBinaryHash { get; } = 8000;

    /// <summary>
    /// Gets a value indicating whether this data provider supports backup
    /// </summary>
    public virtual bool BackupSupported => true;

    #endregion
}