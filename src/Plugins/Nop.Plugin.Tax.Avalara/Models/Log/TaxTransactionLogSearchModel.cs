using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Tax.Avalara.Models.Log
{
    /// <summary>
    /// Represents a tax transaction log search model
    /// </summary>
    public class TaxTransactionLogSearchModel : BaseSearchModel
    {
        #region Properties

        [NopResourceDisplayName("Plugins.Tax.Avalara.Log.Search.CreatedFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedFrom { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Avalara.Log.Search.CreatedTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedTo { get; set; }

        #endregion
    }
}