using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Books
{
    /// <summary>
    /// Represents a book content model
    /// </summary>
    public class BookContentModel: BaseNopModel
    {

        #region Ctor

        public BookContentModel()
        {
            Books = new BookSearchModel(); 
            SearchTitle = new BookSearchModel().SearchTitle;
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.ContentManagement.Book.Books.List.SearchTitle")]
        public string SearchTitle { get; set; }

        public BookSearchModel Books { get; set; } 

        #endregion
    }
}
