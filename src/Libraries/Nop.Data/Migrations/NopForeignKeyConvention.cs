using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Conventions;

namespace Nop.Data.Migrations;

/// <summary>
/// Convention for the default naming of a foreign key
/// </summary>
public class NopForeignKeyConvention : IForeignKeyConvention
{
    #region Fields

    protected readonly INopDataProvider _dataProvider;

    #endregion

    #region Ctor

    public NopForeignKeyConvention(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets the default name of a foreign key
    /// </summary>
    /// <param name="foreignKey">The foreign key definition</param>
    /// <returns>Name of a foreign key</returns>
    protected virtual string GetForeignKeyName(ForeignKeyDefinition foreignKey)
    {
        var foreignColumns = string.Join('_', foreignKey.ForeignColumns);
        var primaryColumns = string.Join('_', foreignKey.PrimaryColumns);

        var keyName = _dataProvider.CreateForeignKeyName(foreignKey.ForeignTable, foreignColumns, foreignKey.PrimaryTable, primaryColumns);

        return keyName;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Applies a convention to a FluentMigrator.Expressions.IForeignKeyExpression
    /// </summary>
    /// <param name="expression">The expression this convention should be applied to</param>
    /// <returns>The same or a new expression. The underlying type must stay the same</returns>
    public IForeignKeyExpression Apply(IForeignKeyExpression expression)
    {
        if (string.IsNullOrEmpty(expression.ForeignKey.Name))
            expression.ForeignKey.Name = GetForeignKeyName(expression.ForeignKey);

        return expression;
    }

    #endregion
}