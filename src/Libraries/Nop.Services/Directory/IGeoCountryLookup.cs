using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Country lookup helper
    /// </summary>
    public partial interface IGeoCountryLookup
    {
        string LookupCountryCode(string str);

        string LookupCountryCode(IPAddress addr);

        string LookupCountryName(string str);

        string LookupCountryName(IPAddress addr);
    }
}