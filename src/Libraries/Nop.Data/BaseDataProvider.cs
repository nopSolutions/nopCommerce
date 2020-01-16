using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Exceptions;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data.Migrations;

namespace Nop.Data
{
    /// <summary>
    /// Represents a base data provider
    /// </summary>
    public abstract partial class BaseDataProvider
    {
        #region Utilities

        /// <summary>
        /// Configure the data context
        /// </summary>
        /// <param name="dataContext">Data context</param>
        protected abstract void ConfigureDataContext(IDataContext dataContext);

        #endregion

        #region Methods

        /// <summary>
        /// Create database schema if it not exists
        /// </summary>
        /// <param name="assembly">Assembly to find the mapping configurations classes;
        /// leave null to search mapping configurations classes on the whole application pull</param>
        public virtual void CreateDatabaseSchemaIfNotExists(Assembly assembly = null)
        {
            DataConnection.DefaultSettings = Singleton<DataSettings>.Instance;

            using (var currentConnection = new NopDataConnection())
            {
                ConfigureDataContext(currentConnection);

                //find database mapping configuration by other assemblies
                var typeFinder = new AppDomainTypeFinder();

                var typeConfigurations = assembly != null
                    ? typeFinder.FindClassesOfType<IMappingConfiguration>(new List<Assembly> {assembly}).ToList()
                    : typeFinder.FindClassesOfType<IMappingConfiguration>().ToList();

                foreach (var typeConfiguration in typeConfigurations)
                {
                    var mappingConfiguration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration);
                    mappingConfiguration.CreateTableIfNotExists(currentConnection);
                }
            }
        }

        /// <summary>
        /// Delete database schema if it exists
        /// </summary>
        /// <param name="assembly">Assembly to find the mapping configurations classes;
        /// leave null to search mapping configurations classes on the whole application pull</param>
        public virtual void DeleteDatabaseSchemaIfExists(Assembly assembly = null)
        {
            //find database mapping configuration by other assemblies
            var typeFinder = new AppDomainTypeFinder();

            var typeConfigurations = assembly != null
                ? typeFinder.FindClassesOfType<IMappingConfiguration>(new List<Assembly> { assembly }).ToList()
                : typeFinder.FindClassesOfType<IMappingConfiguration>().ToList();

            //delete tables
            foreach (var mappingConfiguration in typeConfigurations)
            {
                (EngineContext.Current.ResolveUnregistered(mappingConfiguration) as IMappingConfiguration)
                    ?.DeleteTableIfExists();
            }
        }

        /// <summary>
        /// Executes all found (and unapplied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration;
        /// leave null to search migration on the whole application pull</param>
        public virtual void ApplyUpMigrations(Assembly assembly = null)
        {
            var runner = EngineContext.Current.Resolve<IMigrationRunner>();

            //find database mapping configuration by other assemblies
            var typeFinder = new AppDomainTypeFinder();
            var migrations = (assembly == null ? typeFinder.FindClassesOfType<AutoReversingMigration>() : typeFinder.FindClassesOfType<AutoReversingMigration>(new List<Assembly> { assembly }))
                .Select(migration => migration
                    .GetCustomAttributes(typeof(MigrationAttribute), false)
                    .OfType<MigrationAttribute>()
                    .FirstOrDefault()).Where(migration => migration != null && runner.HasMigrationsToApplyUp(migration.Version)).OrderBy(migration => migration.Version)
                .ToList();

            foreach (var migration in migrations)
            {
                try
                {
                    runner.MigrateUp(migration.Version);
                }
                catch (MissingMigrationsException)
                {
                    // ignore
                }
                catch (Exception ex)
                {
                    if (!(ex.InnerException is SqlException))
                        throw;
                }
            }
        }

        /// <summary>
        /// Executes an Down migration
        /// </summary>
        /// <param name="assembly">Assembly to find the migration;
        /// leave null to search migration on the whole application pull</param>
        public virtual void ApplyDownMigrations(Assembly assembly = null)
        {
            //do not inject services via constructor because it'll cause installation fails
            var migrationRunner = EngineContext.Current.Resolve<IMigrationRunner>();
            var migrationVersionInfoRepository = EngineContext.Current.Resolve<IRepository<MigrationVersionInfo>>();
            var typeFinder = new AppDomainTypeFinder();

            //executes a Down migrations
            foreach (var migration in assembly == null ? typeFinder.FindClassesOfType<Migration>() : typeFinder.FindClassesOfType<Migration>(new[] { assembly }))
            {
                try
                {
                    var migrationAttribute = migration
                        .GetCustomAttributes(typeof(MigrationAttribute), false)
                        .OfType<MigrationAttribute>()
                        .FirstOrDefault();

                    foreach (var migrationVersionInfo in migrationVersionInfoRepository.Table.Where(p => p.Version == migrationAttribute.Version).ToList())
                    {
                        migrationVersionInfoRepository.Delete(migrationVersionInfo);
                    }

                    var downMigration = EngineContext.Current.ResolveUnregistered(migration) as IMigration;
                    migrationRunner.Down(downMigration);
                }
                catch
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// Returns mapped entity descriptor
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <returns>Mapped entity descriptor</returns>
        public virtual EntityDescriptor GetEntityDescriptor<TEntity>() where TEntity : BaseEntity 
        {
            using (var currentConnection = new NopDataConnection())
            {
                return currentConnection.MappingSchema.GetEntityDescriptor(typeof(TEntity));
            }
        }

        /// <summary>
        /// Loads the original copy of the entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Copy of the passed entity</returns>
        public virtual TEntity LoadOriginalCopy<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            using (var currentConnection = new NopDataConnection())
            {
                var entities = currentConnection.GetTable<TEntity>();

                return entities.FirstOrDefault(e => e.Id == Convert.ToInt32(entity.Id));
            }
        }

        /// <summary>
        /// Insert a new entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        public virtual TEntity InsertEntity<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            using (var currentConnection = new NopDataConnection())
            {
                entity.Id = currentConnection.InsertWithInt32Identity(entity);

                return entity;
            }
        }

        /// <summary>
        /// Executes command using System.Data.CommandType.StoredProcedure command type and
        /// returns results as collection of values of specified type
        /// </summary>
        /// <typeparam name="T">Result record type</typeparam>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Returns collection of query result records</returns>
        public virtual IList<T> QueryProc<T>(string procedureName, params DataParameter[] parameters)
        {
            using (var currentConnection = new NopDataConnection())
            {
                return currentConnection.ExecuteStoredProcedure<T>(procedureName, parameters) ?? new List<T>();
            }
        }

        /// <summary>
        /// Executes SQL command and returns results as collection of values of specified type
        /// </summary>
        /// <typeparam name="T">Type of result items</typeparam>
        /// <param name="sql">SQL command text</param>
        /// <param name="parameters">Parameters to execute the SQL command</param>
        /// <returns>Collection of values of specified type</returns>
        public virtual IList<T> Query<T>(string sql, params DataParameter[] parameters)
        {
            using (var currentConnection = new NopDataConnection())
            {
                return currentConnection.Query<T>(sql, parameters)?.ToList() ?? new List<T>();
            }
        }

        #endregion
    }
}