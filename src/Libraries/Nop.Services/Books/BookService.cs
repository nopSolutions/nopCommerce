using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Nop.Core;
using Nop.Core.Domain.Books;
using Nop.Data;
using Nop.Services.Blogs;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Books
{
    /// <summary>
    /// Book service
    /// </summary>
    public partial class BookService : IBookService
    {

        private readonly IRepository<Book> _bookRepository;
        private readonly IEventPublisher _eventPublisher;

        #region Ctor
        public BookService(IRepository<Book> bookRepository,
           IEventPublisher eventPublisher)
        {
            _bookRepository = bookRepository;
            _eventPublisher = eventPublisher;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Get a book by book identifier
        /// </summary>
        /// <param name="bookId">Book identifier</param>
        /// <returns>Book</returns>
        public virtual Book GetBookById(int bookId)
        {
            if (bookId <= 0)
                return null;
            return _bookRepository.ToCachedGetById(bookId);
        }

        /// <summary>
        /// Get all the books 
        /// </summary>
        /// <param name="name">Book name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Books</returns>
        public virtual IPagedList<Book> GetAllBooks(string name = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _bookRepository.Table;
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
                query = query.Where(b => b.Name.Contains(name));

            query = query.OrderByDescending(b => b.CreatedOnUtc);

            var books = new PagedList<Book>(query, pageIndex, pageSize);

            return books;
        }

        /// <summary>
        /// Insert a book
        /// </summary>
        /// <param name="book">Book</param>
        public virtual void InsertBook(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            _bookRepository.Insert(book);

            //event notification
            _eventPublisher.EntityInserted(book);
        }

        /// <summary>
        /// Update a book
        /// </summary>
        /// <param name="book">Book</param>
        public virtual void UpdateBook(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            _bookRepository.Update(book);

            //event notification
            _eventPublisher.EntityUpdated(book);
        }

        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="book">Book</param>
        public virtual void DeleteBook(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            _bookRepository.Delete(book);

            //event notification
            _eventPublisher.EntityDeleted(book);

        }

        #endregion
    }
}
