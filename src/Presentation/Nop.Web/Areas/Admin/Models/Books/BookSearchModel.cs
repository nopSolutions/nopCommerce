using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Books
{
    /// <summary>
    /// Represent a book search parameter
    /// </summary>
    public class BookSearchModel:BaseSearchModel
    {
        #region Properties  
        public string SearchTitle { get; set; } 

        #endregion
    }
}
