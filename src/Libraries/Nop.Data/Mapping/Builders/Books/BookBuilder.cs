using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Books;

namespace Nop.Data.Mapping.Builders.Books
{
    /// <summary>
    /// Represents a book entity builder
    /// </summary>
    public partial class BookBuilder : NopEntityBuilder<Book>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Book.Name)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}