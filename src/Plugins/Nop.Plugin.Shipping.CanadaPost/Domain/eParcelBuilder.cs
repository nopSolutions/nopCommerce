using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Shipping.CanadaPost.Domain
{
    public class eParcelBuilder
    {
        #region Fields
        private readonly Destination _destination;
        private readonly List<Item> _items;
        private readonly Profile _profile;
        private readonly CanadaPostLanguageEnum _language;
        #endregion
        
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="eParcelBuilder"/> class.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="items">The items that will be in the parcel.</param>
        /// <param name="language">The language.</param>
        public eParcelBuilder(Profile profile, Destination destination, List<Item> items, CanadaPostLanguageEnum language)
        {
            this._destination = destination;
            this._items = items;
            this._language = language;
            this._profile = profile;
        }
        #endregion

        #region Methods

        public string GetMessage(bool includeComments)
        {
            var msg = new StringBuilder();
            msg.AppendLine("<?xml version=\"1.0\" ?>");
            msg.AppendLine("<eparcel>");
            // if we want to include the comments in the xml
            if (includeComments)
            {
                msg.AppendLine("<!--********************************-->");
                msg.AppendLine("<!-- Prefered language for the      -->");
                msg.AppendLine("<!-- response (FR/EN)  (optional)   -->");
                msg.AppendLine("<!--********************************-->");
            }
            // set the language
            if (_language == CanadaPostLanguageEnum.French)
                msg.AppendLine("<language>fr</language>");
            else
                msg.AppendLine("<language>en</language>");
            // opening TAG for rates request info
            msg.AppendLine("<ratesAndServicesRequest>");
            // adding information related to the profile of the merchant
            msg.Append(_profile.ToXml(includeComments));
            // if we want to include the comments in the xml
            if (includeComments)
            {
                msg.AppendLine("<!--**********************************-->");
                msg.AppendLine("<!-- List of items in the shopping    -->");
                msg.AppendLine("<!-- cart                             -->");
                msg.AppendLine("<!-- Each item is defined by :        -->");
                msg.AppendLine("<!--   - quantity    (mandatory)      -->");
                msg.AppendLine("<!--   - size        (mandatory)      -->");
                msg.AppendLine("<!--   - weight      (mandatory)      -->");
                msg.AppendLine("<!--   - description (mandatory)      -->");
                msg.AppendLine("<!--   - ready to ship (optional)     -->");
                msg.AppendLine("<!--**********************************-->");
            }
            msg.AppendLine("<lineItems>");
            foreach (Item item in _items)
            {
                // build the item information
                msg.AppendLine(item.ToXml(includeComments));
            }
            msg.AppendLine("</lineItems>");

            // build the destination information
            msg.Append(_destination.ToXml(includeComments));
            // closing TAG for rates request info            
            msg.AppendLine("</ratesAndServicesRequest>");
            msg.AppendLine("</eparcel>");

            return msg.ToString();
        }

        #endregion
    }
}
