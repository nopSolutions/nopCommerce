using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nop.Plugin.Widgets.AbcBonusBundle.Domain
{
    public class ProductAbcBundle : BaseEntity
    {
        public string Sku { get; set; }
        public string ItemNumber { get; set; }
        public string MemoNumber { get; set; }
        public string Comment { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        // This isn't ideal but since the date is stored as a varchar, this will convert appropriately
        public DateTime GetEndDateTime()
        {
            return DateTime.ParseExact(EndDate, "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public static Func<List<ProductAbcBundle>> GetBySkuFunc(IRepository<ProductAbcBundle> repo, string sku)
        {
            return () =>
            {
                var list = repo.Table.Where(p => p.Sku.Trim() == sku).ToList();
                return list;
            };
        }

        public string GetPopupCommand(string path)
        {
            return $"javascript:win = window.open('{path}', 'Bonus Bundle', 'height=500,width=750,top=25,left=25,resizable=yes'); win.focus()";
        }
    }
}
