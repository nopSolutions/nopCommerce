using System;
using System.Linq;
using Nop.Core.Domain.Books;
using Nop.Services.Books;
using Nop.Services.Helpers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Books;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Middle layer between Book UI and DB
    /// </summary>
    public class BookModelFactory : IBookModelFactory
    {

        #region Fields

        private readonly IBookService _bookService;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        public BookModelFactory(IBookService bookService, IDateTimeHelper dateTimeHelper)
        {
            _bookService = bookService;
            _dateTimeHelper = dateTimeHelper;
        }

        #endregion

        /// <summary>
        /// Method to prepare book content model to show book details
        /// </summary>
        /// <param name="bookContentModel">Book content model</param>
        /// <param name="filterByBookId">Book identifier</param>
        /// <returns>Book content model</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual BookContentModel PrepareBookContentModel(BookContentModel bookContentModel, int? filterByBookId)
        {
            if (bookContentModel == null)
                throw new ArgumentNullException(nameof(bookContentModel));

            PrepareBookSearchModel(bookContentModel.Books);
            return bookContentModel;
        }

        /// <summary>
        /// Method to search books using search model 
        /// </summary>
        /// <param name="searchModel">Book search params model</param>
        /// <returns>Books collection</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual BookListModel PrepareBookListModel(BookSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get books
            var books = _bookService.GetAllBooks(searchModel.SearchTitle, searchModel.Page - 1, searchModel.PageSize);

            //prepare list model
            var model = new BookListModel().PrepareToGrid(searchModel, books, () =>
            {
                return books.Select(book =>
                {
                    //fill in model values from the entity
                    var bookModel = book.ToModel<BookModel>();


                    bookModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(book.CreatedOnUtc, DateTimeKind.Utc);

                    return bookModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Method to convert Book details to a model
        /// </summary>
        /// <param name="model">Book model</param>
        /// <param name="book">Book details</param>
        /// <returns>Book model</returns>
        public virtual BookModel PrepareBookModel(BookModel model, Book book)
        {
            //fill in model values from the entity
            if (book != null)
            {
                if (model == null)
                {
                    model = book.ToModel<BookModel>();
                }
                model.CreatedOn = book.CreatedOnUtc;
                model.Name = book.Name;
            }
            return model;
        }

        /// <summary>
        /// Method to prepare search parameters to fecth book details
        /// </summary>
        /// <param name="searchModel">Book search model</param>
        /// <returns>Book search model</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual BookSearchModel PrepareBookSearchModel(BookSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }
    }
}
