
using System.Text;

namespace Nop.Plugin.Shipping.CanadaPost.Domain
{
    public class Profile
    {
        #region Utilities
        /// <summary>
        /// Build an XML string with the destination informations.
        /// </summary>
        /// <param name="includeComments">if set to <c>true</c> [include comments].</param>
        /// <returns></returns>
        internal string ToXml(bool includeComments)
        {
            var xmlString = new StringBuilder();
            

                // if we want to include the comments in the xml
                if (includeComments)
                {
                    xmlString.AppendLine("<!--**********************************-->");
                    xmlString.AppendLine("<!-- Merchant Identification assigned -->");
                    xmlString.AppendLine("<!-- by Canada Post                   -->");
                    xmlString.AppendLine("<!--                                  -->");
                    xmlString.AppendLine("<!-- Note: Use 'CPC_DEMO_HTML' or ask -->");
                    xmlString.AppendLine("<!-- our Help Desk to change your     -->");
                    xmlString.AppendLine("<!-- profile if you want HTML to be   -->");
                    xmlString.AppendLine("<!-- returned to you                  -->");
                    xmlString.AppendLine("<!--**********************************-->");
                }
                // set the merchant Id
                xmlString.AppendLine("<merchantCPCID>" + this.MerchantId + "</merchantCPCID>");
                // if a postal code was specified
                if (this.PostalCode != null)
                {
                    // if we want to include the comments in the xml
                    if (includeComments)
                    {
                        xmlString.AppendLine("<!--*********************************-->");
                        xmlString.AppendLine("<!--Origin Postal Code               -->");
                        xmlString.AppendLine("<!--This parameter is optional       -->");
                        xmlString.AppendLine("<!--*********************************-->");
                    }
                    // set the postal code
                    xmlString.AppendLine("<fromPostalCode>" + this.PostalCode + "</fromPostalCode>");
                }
                if (this.TurnAroundTime != null)
                {
                    // if we want to include the comments in the xml
                    if (includeComments)
                    {
                        xmlString.AppendLine("<!--**********************************-->");
                        xmlString.AppendLine("<!-- Turn Around Time  (hours)        -->");
                        xmlString.AppendLine("<!-- This parameter is optional       -->");
                        xmlString.AppendLine("<!--**********************************-->");
                    }
                    // set the turn around time
                    xmlString.AppendLine("<turnAroundTime> " + this.TurnAroundTime.ToString() + " </turnAroundTime>");
                }
                //msg.AppendLine("<!--**********************************-->");
                //msg.AppendLine("<!-- Total amount in $ of the items   -->");
                //msg.AppendLine("<!-- for insurance calculation        -->");
                //msg.AppendLine("<!-- This parameter is optional       -->");
                //msg.AppendLine("<!--**********************************-->");
                //msg.AppendLine("<itemsPrice>0.00</itemsPrice>");
            
            return xmlString.ToString();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the Merchant Identification assigned by Canada Post.
        /// Note: Use 'CPC_DEMO_HTML' or ask the Help Desk to change the profile if want HTML to be returned
        /// </summary>
        /// <value>The merchant id.</value>
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets the Origin's postal code (optional).
        /// </summary>
        /// <value>The postal code.</value>
        public string PostalCode
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the turn around time (in Hours) (optional).
        /// </summary>
        /// <value>The turn around time.</value>
        public int? TurnAroundTime
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}
