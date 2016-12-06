using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core
{
    public class VendorLite
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static string GetVendorNameFromSession(object sessionObj)
        {
            if (sessionObj==null)
            {
                return string.Empty;
            }
            return (sessionObj as VendorLite).Name;
        }

        public static int GetVendorIdFromSession(object sessionObj)
        {
            if (sessionObj == null)
            {
                return -1;
            }
            return (sessionObj as VendorLite).Id;
        }
    }
}
