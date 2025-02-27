using System.Data;
using System.Data.Common;
using LinqToDB;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.Oracle;
using LinqToDB.SqlQuery;
using Nop.Core;
using Oracle.ManagedDataAccess.Client;

namespace Nop.Data.DataProviders;

public partial class OracleDataProvider : BaseDataProvider, INopDataProvider
{
    #region Fields

    protected static readonly Lazy<IDataProvider> _dataProvider = new(() => OracleTools.GetDataProvider(OracleVersion.v12, OracleProvider.Managed), true);

    #endregion

    #region Utilities

    /// <summary>
    /// Gets a connection to the database for a current data provider
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Connection to a database</returns>
    protected override DbConnection GetInternalDbConnection(string connectionString)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        return new OracleConnection(connectionString);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Performs bulk insert operation for entity collection.
    /// </summary>
    /// <param name="entities">Entities for insert operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task BulkInsertEntitiesAsync<TEntity>(IEnumerable<TEntity> entities)
    {
        using var dataContext = CreateDataConnection(LinqToDbDataProvider);

        foreach (var entity in entities) //we can't use the BulkCopy operation because we don't know the names of sequences in the database
            await InsertEntityAsync(entity);
    }

    /// <summary>
    /// Performs bulk insert operation for entity collection.
    /// </summary>
    /// <param name="entities">Entities for insert operation</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public override void BulkInsertEntities<TEntity>(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
            InsertEntity(entity);
    }

    public void CreateDatabase(string collation, int triesToConnect = 10)
    {
        if (DatabaseExists())
            return;

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

    public Task<int?> GetTableIdentAsync<TEntity>() where TEntity : BaseEntity
    {
        //select SEQUENCE_NAME FROM USER_TAB_IDENTITY_COLS
        throw new NotImplementedException();
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

    public Task BackupDatabaseAsync(string fileName)
    {
        throw new DataException("This database provider does not support backup");
    }

    /// <summary>
    /// Inserts record into table. Returns inserted entity with identity
    /// </summary>
    /// <param name="entity"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns>Inserted entity</returns>
    public override TEntity InsertEntity<TEntity>(TEntity entity)
    {
        using var dataContext = CreateDataConnection();
        try
        {
            entity.Id = dataContext.InsertWithInt32Identity(entity);
        }
        // Ignore when we try insert foreign entity via InsertWithInt32IdentityAsync method
        catch (SqlException ex) when (ex.Message.StartsWith("Identity field must be defined for"))
        {
            dataContext.Insert(entity);
        }

        return entity;
    }

    /// <summary>
    /// Inserts record into table. Returns inserted entity with identity
    /// </summary>
    /// <param name="entity"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the inserted entity
    /// </returns>
    public override async Task<TEntity> InsertEntityAsync<TEntity>(TEntity entity)
    {
        using var dataContext = CreateDataConnection();
        try
        {
            entity.Id = await dataContext.InsertWithInt32IdentityAsync(entity);
        }
        // Ignore when we try insert foreign entity via InsertWithInt32IdentityAsync method
        catch (SqlException ex) when (ex.Message.StartsWith("Identity field must be defined for"))
        {
            await dataContext.InsertAsync(entity);
        }

        return entity;
    }

    public Task RestoreDatabaseAsync(string backupFileName)
    {
        throw new DataException("This database provider does not support backup");
    }

    public Task ReIndexTablesAsync()
    {
        //https://forums.oracle.com/ords/apexds/post/rebuilding-all-indexes-in-a-table-1679
        throw new NotImplementedException();
    }

    public string BuildConnectionString(INopConnectionStringInfo nopConnectionString)
    {
        ArgumentNullException.ThrowIfNull(nopConnectionString);

        if (nopConnectionString.IntegratedSecurity)
            throw new NopException("Data provider supports connection only with login and password");

        var builder = new OracleConnectionStringBuilder
        {
            DataSource = $"(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={nopConnectionString.ServerName})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME={nopConnectionString.ServiceName})))",
            UserID = nopConnectionString.Username,
            Password = nopConnectionString.Password,
        };

        return builder.ConnectionString;
    }

    public Task SetTableIdentAsync<TEntity>(int ident) where TEntity : BaseEntity
    {
        //https://stackoverflow.com/questions/49841132/how-to-find-currval-in-oracle-12c-for-identity-columns
        throw new NotImplementedException();
    }

    #endregion

    #region Properties

    protected override IDataProvider LinqToDbDataProvider => _dataProvider.Value;

    public int SupportedLengthOfBinaryHash => 0;

    public bool BackupSupported => false;

    #endregion
}
