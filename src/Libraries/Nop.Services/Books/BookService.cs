using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
