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
// Contributor(s): 
//------------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.Payment.Methods.eWay
{
    /// <summary>
    /// Summary description for GatewayResponse.
    /// Copyright Web Active Corporation Pty Ltd  - All rights reserved. 1998-2004
    /// This code is for exclusive use with the eWAY payment gateway
    /// </summary>
    public class GatewayResponse
    {

        private int _txAmount = 0;
        private string _txTransactionNumber = "";
        private string _txInvoiceReference = "";
        private string _txOption1 = "";
        private string _txOption2 = "";
        private string _txOption3 = "";
        private bool _txStatus = false;
        private string _txAuthCode = "";
        private string _txError = "";

        /// <summary>
        /// Creates a new instance of the GatewayResponse class from xml
        /// </summary>
        /// <param name="Xml">Xml string</param>
        public GatewayResponse(string Xml)
        {
            string _currentField = "";
            StringReader _sr = new StringReader(Xml);
            XmlTextReader _xtr = new XmlTextReader(_sr);
            _xtr.XmlResolver = null;
            _xtr.WhitespaceHandling = WhitespaceHandling.None;

            // get the root node
            _xtr.Read();

            if ((_xtr.NodeType == XmlNodeType.Element) && (_xtr.Name == "ewayResponse"))
            {
                while (_xtr.Read())
                {
                    if ((_xtr.NodeType == XmlNodeType.Element) && (!_xtr.IsEmptyElement))
                    {
                        _currentField = _xtr.Name;
                        _xtr.Read();
                        if (_xtr.NodeType == XmlNodeType.Text)
                        {
                            switch (_currentField)
                            {
                                case "ewayTrxnError":
                                    _txError = _xtr.Value;
                                    break;

                                case "ewayTrxnStatus":
                                    if (_xtr.Value.ToLower().IndexOf("true") != -1)
                                        _txStatus = true;
                                    break;

                                case "ewayTrxnNumber":
                                    _txTransactionNumber = _xtr.Value;
                                    break;

                                case "ewayTrxnOption1":
                                    _txOption1 = _xtr.Value;
                                    break;

                                case "ewayTrxnOption2":
                                    _txOption2 = _xtr.Value;
                                    break;

                                case "ewayTrxnOption3":
                                    _txOption3 = _xtr.Value;
                                    break;

                                case "ewayReturnAmount":
                                    _txAmount = Int32.Parse(_xtr.Value);
                                    break;

                                case "ewayAuthCode":
                                    _txAuthCode = _xtr.Value;
                                    break;

                                case "ewayTrxnReference":
                                    _txInvoiceReference = _xtr.Value;
                                    break;

                                default:
                                    // unknown field
                                    throw new NopException("Unknown field in response.");
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets a transaction number
        /// </summary>
        public string TransactionNumber
        {
            get { return _txTransactionNumber; }
        }

        /// <summary>
        /// Gets an invoice reference
        /// </summary>
        public string InvoiceReference
        {
            get { return _txInvoiceReference; }
        }
        
        /// <summary>
        /// Gets an option 1
        /// </summary>
        public string Option1
        {
            get { return _txOption1; }
        }

        /// <summary>
        /// Gets an option 2
        /// </summary>
        public string Option2
        {
            get { return _txOption2; }
        }

        /// <summary>
        /// Gets an option 3
        /// </summary>
        public string Option3
        {
            get { return _txOption3; }
        }

        /// <summary>
        /// Gets an authorisatio code
        /// </summary>
        public string AuthorisationCode
        {
            get { return _txAuthCode; }
        }

        /// <summary>
        /// Gets an error 
        /// </summary>
        public string Error
        {
            get { return _txError; }
        }

        /// <summary>
        /// Gets an amount
        /// </summary>
        public int Amount
        {
            get { return _txAmount; }
        }

        /// <summary>
        /// Gets a status
        /// </summary>
        public bool Status
        {
            get { return _txStatus; }
        }
    }
}