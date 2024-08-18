using Nop.Core.Domain.Blogs;
using Nop.Core;
using Nop.Core.Domain.Books;

namespace Nop.Services.Books;
public partial interface IBookService
{
    void InsertBook(Book book);
    void UpdateBook(Book book);
    void DeleteBook(Book book);
    Book GetBookById(int bookId);
    IList<Book> GetAllBooks(string name = null);

    Task<IPagedList<Book>> GetAllBooksAsync(int pageIndex = 0, int pageSize = int.MaxValue, string name = null);
}
