using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Books;

namespace Nop.Data.Mapping.Builders.Books;
public partial class BookMap : NopEntityBuilder<Book>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(Book.Name)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(Book.CreatedOn)).AsDateTime().NotNullable();
    }

    #endregion
}
