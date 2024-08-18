using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Blogs;
using Nop.Core;
using Nop.Core.Domain.Books;
using Nop.Data;

namespace Nop.Services.Books;
public partial class BookService: IBookService
{
    private readonly IRepository<Book> _bookRepository;

    public BookService(IRepository<Book> bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public void InsertBook(Book book)
    {
        _bookRepository.Insert(book);
    }

    public void UpdateBook(Book book)
    {
        _bookRepository.Update(book);
    }

    public void DeleteBook(Book book)
    {
        _bookRepository.Delete(book);
    }

    public Book GetBookById(int bookId)
    {
        return _bookRepository.GetById(bookId);
    }

    public IList<Book> GetAllBooks(string name = null)
    {
        var query = _bookRepository.Table;
        if (!string.IsNullOrEmpty(name))
            query = query.Where(b => b.Name.Contains(name));
        return query.ToList();
    }

    public virtual async Task<IPagedList<Book>> GetAllBooksAsync(int pageIndex = 0, int pageSize = int.MaxValue, string name = null)
    {
        return await _bookRepository.GetAllPagedAsync(query =>
        {
            if (!string.IsNullOrEmpty(name))
                query = query.Where(b => b.Name.Contains(name));

            query = query.OrderByDescending(b => b.Id);

            return query;
        }, pageIndex, pageSize);
    }
}
