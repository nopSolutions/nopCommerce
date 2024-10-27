using System;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Books
{
    /// <summary>
    /// Represents a book
    /// </summary>
    public partial class BookModel: BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.ContentManagement.Book.Books.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Book.Books.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        #endregion
    }
}
