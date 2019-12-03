using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Exceptions;
using LinqToDB;
using LinqToDB.Data;
using Nop.Core;
using Nop.Core.Infrastructure;

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

        /// <summary>
        /// Get DB parameter
        /// </summary>
        /// <param name="dbType">Data type</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        protected virtual DataParameter GetParameter(DataType dbType, string parameterName, object parameterValue)
        {
            var parameter = new DataParameter
            {
                Name = parameterName,
                Value = parameterValue,
                DataType = dbType
            };

            return parameter;
        }

        /// <summary>
        /// Get output DB parameter
        /// </summary>
        /// <param name="dbType">Data type</param>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Parameter</returns>
        protected virtual DataParameter GetOutputParameter(DataType dbType, string parameterName)
        {
            var parameter = new DataParameter
            {
                Name = parameterName,
                DataType = dbType,
                Direction = ParameterDirection.Output
            };

            return parameter;
        }

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
                    continue;
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

        /// <summary>
        /// Get string parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public virtual DataParameter GetStringParameter(string parameterName, string parameterValue)
        {
            return GetParameter(DataType.NVarChar, parameterName, (object)parameterValue ?? DBNull.Value);
        }

        /// <summary>
        /// Get output string parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Parameter</returns>
        public virtual DataParameter GetOutputStringParameter(string parameterName)
        {
            return GetOutputParameter(DataType.NVarChar, parameterName);
        }

        /// <summary>
        /// Get int parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public virtual DataParameter GetInt32Parameter(string parameterName, int? parameterValue)
        {
            return GetParameter(DataType.Int32, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
        }

        /// <summary>
        /// Get output int32 parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Parameter</returns>
        public virtual DataParameter GetOutputInt32Parameter(string parameterName)
        {
            return GetOutputParameter(DataType.Int32, parameterName);
        }

        /// <summary>
        /// Get boolean parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public virtual DataParameter GetBooleanParameter(string parameterName, bool? parameterValue)
        {
            return GetParameter(DataType.Boolean, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
        }

        /// <summary>
        /// Get decimal parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public virtual DataParameter GetDecimalParameter(string parameterName, decimal? parameterValue)
        {
            return GetParameter(DataType.Decimal, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
        }

        /// <summary>
        /// Get datetime parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public virtual DataParameter GetDateTimeParameter(string parameterName, DateTime? parameterValue)
        {
            return GetParameter(DataType.DateTime, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
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

        public virtual IEnumerable<T> QueryProc<T>(string sql, params DataParameter[] parameters)
        {
            if (_dataConnection == null && !DataSettingsManager.DatabaseIsInstalled)
                _dataConnection = new NopDataConnection();

            return _dataConnection.QueryProc<T>(sql, parameters);
        }

        public virtual IEnumerable<T> Query<T>(string sql)
        {
            if (_dataConnection == null && !DataSettingsManager.DatabaseIsInstalled)
                _dataConnection = new NopDataConnection();

            return _dataConnection.Query<T>(sql);
        }

        public virtual int Execute(string sql, params DataParameter[] parameters)
        {
            if(_dataConnection == null && !DataSettingsManager.DatabaseIsInstalled)
                _dataConnection = new NopDataConnection();

            return _dataConnection.Execute(sql, parameters);
        }

        #endregion
    }
}