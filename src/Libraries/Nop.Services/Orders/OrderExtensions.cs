using System;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    public static class OrderExtensions
    {
        /// <summary>
        /// Formats the order note text
        /// </summary>
        /// <param name="orderNote">Order note</param>
        /// <returns>Formatted text</returns>
        public static string FormatOrderNoteText(this OrderNote orderNote)
        {
            if (orderNote == null)
                throw new ArgumentNullException("orderNote");

            string text = orderNote.Note;

            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = Nop.Core.Html.HtmlHelper.FormatText(text, false, true, false, false, false, false);

            return text;
        }
    }
}
