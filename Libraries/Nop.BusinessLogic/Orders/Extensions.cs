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
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.Common.Utils.Html;


namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Formats the order note text
        /// </summary>
        /// <param name="orderNote">Order note</param>
        /// <returns>Formatted text</returns>
        public static string FormatOrderNoteText(this OrderNote orderNote)
        {
            if (orderNote == null || String.IsNullOrEmpty(orderNote.Note))
                return string.Empty;

            string result = HtmlHelper.FormatText(orderNote.Note, false, true, false, false, false, false);
            return result;
        }

        /// <summary>
        ///  Formats the comments text of a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <returns>Formatted text</returns>
        public static string FormatReturnRequestCommentsText(this ReturnRequest returnRequest)
        {
            if (returnRequest == null || String.IsNullOrEmpty(returnRequest.CustomerComments))
                return string.Empty;

            string result = HtmlHelper.FormatText(returnRequest.CustomerComments, false, true, false, false, false, false);
            return result;
        }
    }
}
