using FluentMigrator;
using FluentMigrator.Runner.Generators;

namespace Nop.Data.Migrations;

/// <summary>
/// An <see cref="IGeneratorAccessor"/> implementation that selects one generator by data settings
/// </summary>
public class NopGeneratorAccessor : IGeneratorAccessor
{
    #region Ctor

    public NopGeneratorAccessor(IEnumerable<IMigrationGenerator> generators)
    {
        ConfigureGenerator(generators.ToList());
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Configure generator
    /// </summary>
    /// <param name="generators">Collection of migration generators</param>
    protected virtual void ConfigureGenerator(IList<IMigrationGenerator> generators)
    {
        var dataSettings = DataSettingsManager.LoadSettings();

        if (!generators.Any())
            throw new InvalidOperationException("No migration generator registered.");

        Generator = dataSettings is null ? generators.FirstOrDefault() : dataSettings.DataProvider switch
        {
            DataProviderType.SqlServer => FindGenerator(generators, GeneratorIdConstants.SqlServer),
            DataProviderType.MySql => FindGenerator(generators, GeneratorIdConstants.MySql5),
            DataProviderType.PostgreSQL => FindGenerator(generators, GeneratorIdConstants.PostgreSQL9_2),
            _ => throw new InvalidOperationException(
                $@"A migration generator for Data provider type {dataSettings.DataProvider} couldn't be found.")
        };
    }

    /// <summary>
    /// Gets single processor by DatabaseType or DatabaseTypeAlias
    /// </summary>
    /// <param name="generators">Collection of migration generators</param>
    /// <param name="generatorId">The ID of the generator</param>
    /// <returns></returns>
    protected IMigrationGenerator FindGenerator(IList<IMigrationGenerator> generators,
        string generatorId)
    {
        if (generators.FirstOrDefault(p =>
                p.GeneratorId.Equals(generatorId, StringComparison.OrdinalIgnoreCase) ||
                p.GeneratorIdAliases.Any(a => a.Equals(generatorId, StringComparison.OrdinalIgnoreCase))) is
            IMigrationGenerator processor)
        {
            return processor;
        }

        var generatorNames = string.Join(", ",
            generators.Select(p => p.GeneratorId).Union(generators.SelectMany(p => p.GeneratorIdAliases)));

        throw new InvalidOperationException(
            $@"A migration generator with the ID {generatorId} couldn't be found. Available generators are: {generatorNames}");
    }

    #endregion

    #region  Properties

    public IMigrationGenerator Generator { get; protected set; }

    #endregion
}