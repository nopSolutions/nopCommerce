using System;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.AbcCore.Areas.Admin.PageNotFound
{
    public partial record PageNotFoundModel : BaseNopEntityModel
    {
        public string Slug { get; set; }
        public string ReferrerUrl { get; set; }
        public int CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime Date { get; set; }
        public string IpAddress { get; set; }
    }
}