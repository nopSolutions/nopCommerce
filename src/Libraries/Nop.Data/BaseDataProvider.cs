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
        #region Ctor

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

        protected virtual void CreateDatabaseSchemaIfNotExists()
        {
            //find database mapping configuration by other assemblies
            var typeFinder = new AppDomainTypeFinder();
            var typeConfigurations = typeFinder.FindClassesOfType<IMappingConfiguration>().ToList();

            foreach (var typeConfiguration in typeConfigurations)
            {
                var mappingConfiguration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration);
                mappingConfiguration.CreateTableIfNotExists();
            }
        }

        protected virtual void ApplyMigrations()
        {
            //find database mapping configuration by other assemblies
            var typeFinder = new AppDomainTypeFinder();
            var migrations = typeFinder.FindClassesOfType<AutoReversingMigration>().ToList();
            var runner = EngineContext.Current.Resolve<IMigrationRunner>();

            foreach (var migration in migrations)
            {
                var migrationAttribute = migration
                    .GetCustomAttributes(typeof(MigrationAttribute), false)
                    .OfType<MigrationAttribute>()
                    .FirstOrDefault();

                try
                {
                    if (migrationAttribute == null || !runner.HasMigrationsToApplyUp(migrationAttribute.Version))
                        continue;

                    runner.MigrateUp(migrationAttribute.Version);
                }
                catch (MissingMigrationsException)
                {
                }
                catch (Exception ex)
                {
                    if (!(ex.InnerException is SqlException))
                        throw;
                }
            }
        }

        #endregion

        #region Methods

        //TODO: 239 need to move some other place
        public virtual void DeletePluginData(Type pluginType)
        {
            //do not inject services via constructor because it'll cause installation fails
            var migrationRunner = EngineContext.Current.Resolve<IMigrationRunner>();
            var migrationVersionInfoRepository = EngineContext.Current.Resolve<IRepository<MigrationVersionInfo>>();
            var typeFinder = new AppDomainTypeFinder();

            //executes a plugin Down migrations
            foreach (var migration in typeFinder.FindClassesOfType<Migration>(new[] { Assembly.GetAssembly(pluginType) }))
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

            //delete plugin tables
            foreach (var mappingConfiguration in typeFinder.FindClassesOfType<IMappingConfiguration>(new[] { Assembly.GetAssembly(pluginType) }))
            {
                try
                {
                    (EngineContext.Current.ResolveUnregistered(mappingConfiguration) as IMappingConfiguration)?.DeleteTableIfExists();
                }
                catch
                {
                    // ignored
                }
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