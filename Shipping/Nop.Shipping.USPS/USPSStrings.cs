//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): RJH 08/07/2009. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;

namespace NopSolutions.NopCommerce.Shipping.Methods.USPS
{
    /// <summary>
    /// Class for USPS V3 XML rate class 
    /// </summary>
    public class USPSStrings
    {
        /// <summary>
        /// String array field instance.
        /// </summary>
        /// V3 USPS Service must be Express, Express SH, Express Commercial, Express SH Commercial, First Class, Priority, Priority Commercial, Parcel, Library, BPM, Media, ALL or ONLINE;
        /// Comment out the services not needed. 
        string[] _elements = {  // "Express", 
                                    // "Express SH",
                                    // "Express Commercial",
                                    // "Express SH Commercial",
                                    // "First Class",                       /* 13 oz limit */
                                    // "Priority",
                                    // "Priority Commercial",
                                    // "Parcel", 
                                    // "Library",
                                    // "BPM",
                                    // "Media",
                                    "ALL",
                                    // "ONLINE" 
                                 };

        /// <summary>
        /// String array property getter.
        /// </summary>
        public string[] Elements
        {
            get { return _elements; }
        }

        /// <summary>
        /// String array indexer.
        /// </summary>
        public string this[int index]
        {
            get { return _elements[index]; }
        }
    }
}
