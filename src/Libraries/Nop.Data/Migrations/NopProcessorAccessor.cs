using System;
using System.Collections.Generic;
using System.Linq;
using FluentMigrator;
using FluentMigrator.Exceptions;
using FluentMigrator.Runner.Processors;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// An <see cref="IProcessorAccessor"/> implementation that selects one generator by data settings
    /// </summary>
    public class NopProcessorAccessor : IProcessorAccessor
    {
        #region Ctor

        public NopProcessorAccessor(IEnumerable<IMigrationProcessor> processors)
        {
            var dataSettings = DataSettingsManager.LoadSettings();

            var procs = processors.ToList();
            if (procs.Count == 0)
                throw new ProcessorFactoryNotFoundException("No migration processor registered.");

            if (dataSettings is null)
                Processor = procs.FirstOrDefault();
            else
            {
                switch (dataSettings.DataProvider)
                {
                    case DataProviderType.SqlServer:
                        Processor = FindGenerator(procs, "SqlServer");
                        break;
                    case DataProviderType.MySql:
                        Processor = FindGenerator(procs, "MySQL");
                        break;
                    default:
                        throw new ProcessorFactoryNotFoundException($@"A migration generator for Data provider type {dataSettings.DataProvider} couldn't be found.");
                }
            }
        }

        #endregion

        #region Utils

        /// <summary>
        /// Gets single processor by DatabaseType or DatabaseTypeAlias
        /// </summary>
        /// <param name="processors">Collection of migration processors</param>
        /// <param name="processorsId">DatabaseType or DatabaseTypeAlias</param>
        /// <returns></returns>
        private IMigrationProcessor FindGenerator(IReadOnlyCollection<IMigrationProcessor> processors,
            string processorsId)
        {
            if (processors.FirstOrDefault(p =>
                    p.DatabaseType.Equals(processorsId, StringComparison.OrdinalIgnoreCase) ||
                    p.DatabaseTypeAliases.Any(a => a.Equals(processorsId, StringComparison.OrdinalIgnoreCase))) is
                IMigrationProcessor processor)
                return processor;

            var generatorNames = string.Join(", ",
                processors.Select(p => p.DatabaseType).Union(processors.SelectMany(p => p.DatabaseTypeAliases)));

            throw new ProcessorFactoryNotFoundException(
                $@"A migration generator with the ID {processorsId} couldn't be found. Available generators are: {generatorNames}");
        }

        #endregion

        #region  Properties

        public IMigrationProcessor Processor { get; }

        #endregion
    }
}