using FluentMigrator;
using FluentMigrator.Exceptions;
using FluentMigrator.Runner.Processors;

namespace Nop.Data.Migrations;

/// <summary>
/// An <see cref="IProcessorAccessor"/> implementation that selects one processor by name
/// </summary>
public class NopProcessorAccessor : IProcessorAccessor
{
    #region Ctor

    public NopProcessorAccessor(IEnumerable<IMigrationProcessor> processors)
    {
        ConfigureProcessor(processors.ToList());
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Configure processor
    /// </summary>
    /// <param name="processors">Collection of migration processors</param>
    protected virtual void ConfigureProcessor(IList<IMigrationProcessor> processors)
    {
        var dataSettings = DataSettingsManager.LoadSettings();

        if (!processors.Any())
            throw new ProcessorFactoryNotFoundException("No migration processor registered.");

        Processor = dataSettings is null ? processors.FirstOrDefault() : dataSettings.DataProvider switch
        {
            DataProviderType.SqlServer => FindProcessor(processors, ProcessorIdConstants.SqlServer),
            DataProviderType.MySql => FindProcessor(processors, ProcessorIdConstants.MySql5),
            DataProviderType.PostgreSQL => FindProcessor(processors, ProcessorIdConstants.PostgreSQL92),
            _ => throw new ProcessorFactoryNotFoundException(
                $@"A migration processor for Data provider type {dataSettings.DataProvider} couldn't be found.")
        };
    }

    /// <summary>
    /// Gets single processor by DatabaseType or DatabaseTypeAlias
    /// </summary>
    /// <param name="processors">Collection of migration processors</param>
    /// <param name="processorsId">DatabaseType or DatabaseTypeAlias</param>
    /// <returns></returns>
    protected IMigrationProcessor FindProcessor(IList<IMigrationProcessor> processors,
        string processorsId)
    {
        if (processors.FirstOrDefault(p =>
                p.DatabaseType.Equals(processorsId, StringComparison.OrdinalIgnoreCase) ||
                p.DatabaseTypeAliases.Any(a => a.Equals(processorsId, StringComparison.OrdinalIgnoreCase))) is
            IMigrationProcessor processor)
        {
            return processor;
        }

        var generatorNames = string.Join(", ",
            processors.Select(p => p.DatabaseType).Union(processors.SelectMany(p => p.DatabaseTypeAliases)));

        throw new ProcessorFactoryNotFoundException(
            $@"A migration processor with the ID {processorsId} couldn't be found. Available generators are: {generatorNames}");
    }

    #endregion

    #region  Properties

    public IMigrationProcessor Processor { get; protected set; }

    #endregion
}