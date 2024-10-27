using Nop.Core;
using Nop.Core.Domain.Books;

namespace Nop.Services.Books
{
    /// <summary>
    /// Book service interface
    /// </summary>
    public partial interface IBookService
    {

        /// <summary>
        /// Get a book by book identifier
        /// </summary>
        /// <param name="bookId">Book identifier</param>
        /// <returns>Book</returns>
        Book GetBookById(int bookId);

        /// <summary>
        /// Get all the books 
        /// </summary>
        /// <param name="name">Book name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Books</returns>
        IPagedList<Book> GetAllBooks(string name = null, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Insert a book
        /// </summary>
        /// <param name="book">Book</param>
        void InsertBook(Book book);

        /// <summary>
        /// Update a book
        /// </summary>
        /// <param name="book">Book</param>
        void UpdateBook(Book book);

        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="book">Book</param>
        void DeleteBook(Book book);
    }
}
