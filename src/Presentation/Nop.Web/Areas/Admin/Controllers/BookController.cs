using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Books;
using Nop.Services.Books;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Areas.Admin.Models.Books;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for Books
    /// </summary>
    public partial class BookController : BaseAdminController
    {
        #region Fields

        private readonly IBookModelFactory _bookModelFactory;
        private readonly IBookService _bookService;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public BookController(IBookModelFactory bookModelFactory,
            IBookService bookService,
            IPermissionService permissionService,
            INotificationService notificationService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService)
        {
            _bookModelFactory = bookModelFactory;
            _bookService = bookService;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
        }


        #endregion


        #region Methods

        /// <summary>
        /// Index page of Book
        /// </summary>
        /// <returns></returns>
        public virtual IActionResult Index()
        {
            return View("Books");
        }

        /// <summary>
        /// Get Book by book identifier
        /// </summary>
        /// <param name="filterByBookId"></param>
        /// <returns></returns>
        public virtual IActionResult Books(int? filterByBookId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBook))
                return AccessDeniedView();

            //prepare model
            var model = _bookModelFactory.PrepareBookContentModel(new BookContentModel(), filterByBookId);

            return View(model);
        }

        /// <summary>
        /// Get list of books
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual IActionResult List(BookSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBook))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _bookModelFactory.PrepareBookListModel(searchModel);

            return Json(model);
        }

        /// <summary>
        /// Get book create page
        /// </summary>
        /// <returns></returns>
        public virtual IActionResult BookCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBook))
                return AccessDeniedView();

            //prepare model
            var model = _bookModelFactory.PrepareBookModel(new BookModel(), null);

            return View(model);
        }

        /// <summary>
        /// Method to create a book with book details
        /// </summary>
        /// <param name="model">book details</param> 
        /// <returns></returns> 
        [HttpPost]
        public virtual IActionResult BookCreate(BookModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBook))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var book = model.ToEntity<Book>();
                book.CreatedOnUtc = DateTime.UtcNow;
                _bookService.InsertBook(book);

                //activity log
                _customerActivityService.InsertActivity("AddNewBook",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewBook"), book.Id), book);


                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Book.Books.Added"));

                return RedirectToAction("Books");
            }

            //prepare model
            model = _bookModelFactory.PrepareBookModel(model, null);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Get book edit page
        /// </summary>
        /// <param name="id">Book identifier</param>
        /// <returns></returns>
        public virtual IActionResult BookEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBook))
                return AccessDeniedView();

            //try to get a book with the specified id
            var book = _bookService.GetBookById(id);
            if (book == null)
                return RedirectToAction("Books");

            //prepare model
            var model = _bookModelFactory.PrepareBookModel(null, book);

            return View(model);
        }

        /// <summary>
        /// Update edited book details
        /// </summary>
        /// <param name="model">Book details</param>
        /// <returns></returns>
        [HttpPost]
        public virtual IActionResult BookEdit(BookModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBook))
                return AccessDeniedView();

            //try to get a book with the specified id
            var book = _bookService.GetBookById(model.Id);
            if (book == null)
                return RedirectToAction("Books");

            if (ModelState.IsValid)
            {
                book = model.ToEntity(book);
                _bookService.UpdateBook(book);

                //activity log
                _customerActivityService.InsertActivity("EditBook",
                    string.Format(_localizationService.GetResource("ActivityLog.EditBook"), book.Id), book);



                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Book.Books.Updated"));
                return RedirectToAction("Books");
            }

            //prepare model
            model = _bookModelFactory.PrepareBookModel(model, book);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param name="id">Book identifier</param>
        /// <returns></returns>
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBook))
                return AccessDeniedView();

            //try to get a book with the specified id
            var book = _bookService.GetBookById(id);
            if (book == null)
                return RedirectToAction("Books");

            _bookService.DeleteBook(book);

            //activity log
            _customerActivityService.InsertActivity("DeleteBook",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteBook"), book.Id), book);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Book.Books.Deleted"));

            return RedirectToAction("Books");
        }

        #endregion
    }
}