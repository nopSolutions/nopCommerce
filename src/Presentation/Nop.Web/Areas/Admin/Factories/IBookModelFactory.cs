using Nop.Core.Domain.Books;
using Nop.Web.Areas.Admin.Models.Books;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Middle layer between Book UI and DB - interafce
    /// </summary>
    public interface IBookModelFactory
    {
        /// <summary>
        /// Method to prepare book content model to show book details
        /// </summary>
        /// <param name="bookContentModel">Book content model</param>
        /// <param name="filterByBookId">Book identifier</param>
        /// <returns>Book content model</returns>
        BookContentModel PrepareBookContentModel(BookContentModel bookContentModel, int? filterByBookId);

        /// <summary>
        /// Method to prepare search parameters to fecth book details
        /// </summary>
        /// <param name="searchModel">Book search model</param>
        /// <returns>Book search model</returns>
        BookSearchModel PrepareBookSearchModel(BookSearchModel searchModel);

        /// <summary>
        /// Method to search books using search model 
        /// </summary>
        /// <param name="searchModel">Book search params model</param>
        /// <returns>Books collection</returns>
        BookListModel PrepareBookListModel(BookSearchModel searchModel);

        /// <summary>
        /// Method to convert Book details to a model
        /// </summary>
        /// <param name="model">Book model</param>
        /// <param name="book">Book details</param>
        /// <returns>Book model</returns> 
        BookModel PrepareBookModel(BookModel model, Book book);
    }
}
