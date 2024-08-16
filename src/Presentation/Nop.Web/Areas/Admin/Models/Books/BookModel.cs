using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Books;

public partial record BookModel: BaseNopEntityModel
{
    public string Name { get; set; }
    public DateTime CreatedOn { get; set; }
}
