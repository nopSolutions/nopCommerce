using AO.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AO.Services.Domain
{
    public class AOInvoiceListModel
    {
        public IList<InvoiceModel> InvoiceList { get; set; }        

        public string SearchPhrase { get; set; }

        public string ErrorMessage { get; set; }
    }
}
