using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.AbcCore.Models
{
    public class AbcCategoryDetailsModel
    {
        public int CategoryId { get; set; }

        [UIHint("Picture")]
        public int HawthornePictureId { get; set; }
    }
}
