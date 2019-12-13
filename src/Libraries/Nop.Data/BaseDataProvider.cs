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
        #region Fields

        protected DataConnection _dataConnection;

        #endregion

        #region Ctor

        protected BaseDataProvider()
        {
            if (!DataSettingsManager.DatabaseIsInstalled)
                return;

            _dataConnection = new NopDataConnection();
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        public virtual void CreateDatabaseSchemaIfNotExists(Assembly assembly = null)
        {
            //find database mapping configuration by other assemblies
            var typeFinder = new AppDomainTypeFinder();

            var typeConfigurations = assembly != null
                ? typeFinder.FindClassesOfType<IMappingConfiguration>(new List<Assembly> { assembly }).ToList()
                : typeFinder.FindClassesOfType<IMappingConfiguration>().ToList();

            foreach (var typeConfiguration in typeConfigurations)
            {
                var mappingConfiguration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration);
                mappingConfiguration.CreateTableIfNotExists();
            }
        }

        public virtual void DeleteDatabaseSchemaIfNotExists(Assembly assembly = null)
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

        public virtual EntityDescriptor GetEntityDescriptor<TEntity>() where TEntity : BaseEntity 
        {
            return _dataConnection.MappingSchema.GetEntityDescriptor(typeof(TEntity));
        }

        /// <summary>
        /// Loads the original copy of the entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Copy of the passed entity</returns>
        public virtual TEntity LoadOriginalCopy<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            var entities = _dataConnection.GetTable<TEntity>();
            return entities.FirstOrDefault(e => e.Id == Convert.ToInt32(entity.Id));
        }

        /// <summary>
        /// Insert a new entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        public virtual TEntity InsertEntity<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            if (_dataConnection == null && !DataSettingsManager.DatabaseIsInstalled)
                _dataConnection = new NopDataConnection();

            entity.Id = _dataConnection.InsertWithInt32Identity(entity);

            return entity;
        }

        public virtual IEnumerable<T> QueryProc<T>(string procedureName, params DataParameter[] parameters)
        {
            if (_dataConnection == null && !DataSettingsManager.DatabaseIsInstalled)
                _dataConnection = new NopDataConnection();

            return _dataConnection.QueryProc<T>(procedureName, parameters);
        }

        public virtual IEnumerable<T> Query<T>(string sql, params DataParameter[] parameters)
        {
            if (_dataConnection == null && !DataSettingsManager.DatabaseIsInstalled)
                _dataConnection = new NopDataConnection();

            return _dataConnection.Query<T>(sql, parameters);
        }

        #endregion
    }
}