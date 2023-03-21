using System.IO;
using System.Xml;
using Nop.Core;

namespace Nop.Plugin.Payments.Param
{
    /// <summary>
    /// Summary description for GatewayResponse.
    /// Copyright Web Active Corporation Pty Ltd  - All rights reserved. 1998-2004
    /// This code is for exclusive use with the Param payment gateway
    /// </summary>
    public class GatewayResponse
    {
        /// <summary>
        /// Creates a new instance of the GatewayResponse class from xml
        /// </summary>
        /// <param name="xml">Xml string</param>
        public GatewayResponse(string xml)
        {
            var sr = new StringReader(xml);
            var xtr = new XmlTextReader(sr)
            {
                XmlResolver = null,
                WhitespaceHandling = WhitespaceHandling.None
            };

            // get the root node
            xtr.Read();

            if (xtr.NodeType != XmlNodeType.Element || xtr.Name != "ewayResponse") return;

            while (xtr.Read())
            {
                if (xtr.NodeType != XmlNodeType.Element || xtr.IsEmptyElement) continue;

                var currentField = xtr.Name;
                xtr.Read();
                if (xtr.NodeType != XmlNodeType.Text) continue;

                switch (currentField)
                {
                    case "ewayTrxnError":
                        Error = xtr.Value;
                        break;

                    case "ewayTrxnStatus":
                        if (xtr.Value.ToLower().IndexOf("true") != -1)
                            Status = true;
                        break;

                    case "ewayTrxnNumber":
                        TransactionNumber = xtr.Value;
                        break;

                    case "ewayTrxnOption1":
                        Option1 = xtr.Value;
                        break;

                    case "ewayTrxnOption2":
                        Option2 = xtr.Value;
                        break;

                    case "ewayTrxnOption3":
                        Option3 = xtr.Value;
                        break;

                    case "ewayReturnAmount":
                        Amount = int.Parse(xtr.Value);
                        break;

                    case "ewayAuthCode":
                        AuthorisationCode = xtr.Value;
                        break;

                    case "ewayTrxnReference":
                        InvoiceReference = xtr.Value;
                        break;

                    default:
                        // unknown field
                        throw new NopException("Unknown field in response.");
                }
            }
        }

        /// <summary>
        /// Gets a transaction number
        /// </summary>
        public string TransactionNumber { get; }

        /// <summary>
        /// Gets an invoice reference
        /// </summary>
        public string InvoiceReference { get; }

        /// <summary>
        /// Gets an option 1
        /// </summary>
        public string Option1 { get; }

        /// <summary>
        /// Gets an option 2
        /// </summary>
        public string Option2 { get; }

        /// <summary>
        /// Gets an option 3
        /// </summary>
        public string Option3 { get; }

        /// <summary>
        /// Gets an authorisatio code
        /// </summary>
        public string AuthorisationCode { get; }

        /// <summary>
        /// Gets an error 
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Gets an amount
        /// </summary>
        public int Amount { get; }

        /// <summary>
        /// Gets a status
        /// </summary>
        public bool Status { get; }
    }
}
