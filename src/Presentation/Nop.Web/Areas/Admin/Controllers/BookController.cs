using Microsoft.AspNetCore.Mvc;
using Nop.Services.Books;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Books;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Controllers;

public class BookController : BaseAdminController
{
    protected readonly IBookService _bookService;
    protected readonly ILocalizationService _localizationService;

    public BookController(IBookService bookService, ILocalizationService localizationService)
    {
        _bookService = bookService;
        _localizationService = localizationService;
    }

    public IActionResult List()
    {
        var model = new BookContentModel();
        model.Book.AvailablePageSizes = "10, 20, 50, 100";
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> List(BookSearchModel searchModel)
    {
        var books = await _bookService.GetAllBooksAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,name:searchModel.SearchName);

        //prepare list model
        var model = new BookListModel().PrepareToGrid(searchModel, books, () =>
        {
            return books.Select(book =>
            {
                //fill in model values from the entity
                var bookModel = book.ToModel();
                return bookModel;
            });
        });

        return Json(model);
    }

    #region Create / Edit / Delete

    public virtual IActionResult Create()
    {
        //prepare model
        var model = new BookModel();
        return View(model);
    }

    [HttpPost]
    public IActionResult Create(BookModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var book = model.ToEntity();
        book.CreatedOn = DateTime.UtcNow;
        _bookService.InsertBook(book);

        return RedirectToAction("List");
    }

    public IActionResult Edit(int id)
    {
        //try to get a category with the specified id
        var book = _bookService.GetBookById(id);
        if (book == null)
            return RedirectToAction("List");

        //prepare model
        var model = book.ToModel();
        return View(model);
    }

    [HttpPost]
    public IActionResult Edit(BookModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var book = model.ToEntity();
        _bookService.UpdateBook(book);

        return RedirectToAction("List");
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        //try to get a book with the specified id
        var book = _bookService.GetBookById(id);
        if (book == null)
            return RedirectToAction("List");

        _bookService.DeleteBook(book);

        //activity log
        return RedirectToAction("List");
    }

    #endregion
}
