using Nop.Plugin.POS.Kaching.Models.ReconciliationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Admin.Accounting.Models
{
    public class ReconciliationModel
    {
        public IList<Dictionary<string, Reconciliation>> ReconciliationList2 { get; set; }

        public IList<Dictionary<string, Reconciliation>> ReconciliationList { get; set; }

        public int Count { get; set; }

        public DateTime dtFrom { get; set; }

        public DateTime dtTo { get; set; }

        public string ErrorMessage { get; set; }
    }
}
