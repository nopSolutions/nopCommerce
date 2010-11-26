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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace NopSolutions.NopCommerce.Payment.Methods.SecurePay
{
    /* SecureXML.Transaction
     * 
     * Handles SecurePay credit-card transactions.
     * 
     * It supports the following transaction types:
     *  Credit Payment (standard)
     *  Credit Refund
     *  Credit Reversal
     *  Credit Preauthorisation
     *  Credit Preauthorised completion (Advice)
     *  Add Trigger/Peridic Payment
     *  Delete Trigger/Periodic Payment
     *  Trigger Triggered payment
     */
    public class XmlTransaction
    {
        /* Modes */
        public const int MODE_TEST = 1;
        public const int MODE_LIVE = 2;
        public const int MODE_PERIODIC_TEST = 3;
        public const int MODE_PERIODIC_LIVE = 4;
        public const int MODE_FRAUD_TEST = 5;
        public const int MODE_FRAUD_LIVE = 6;

        /* Server URLs */
        public const string URL_TEST = "https://test.securepay.com.au/xmlapi/payment";
        public const string URL_LIVE = "https://www.securepay.com.au/xmlapi/payment";
        public const string URL_PERIODIC_TEST = "https://test.securepay.com.au/xmlapi/periodic";
        public const string URL_PERIODIC_LIVE = "https://www.securepay.com.au/xmlapi/periodic";
        public const string URL_FRAUD_TEST = "https://www.securepay.com.au/antifraud_test/payment";
        public const string URL_FRAUD_LIVE = "https://www.securepay.com.au/antifraud/payment";

        /* Transaction types. */
        public const int TXN_STANDARD = 0;
        public const int TXN_PERIODIC = 3;
        public const int TXN_REFUND = 4;
        public const int TXN_REVERSE = 6;
        public const int TXN_PREAUTH = 10;
        public const int TXN_ADVICE = 11;
        public const int TXN_RECURRING = 14;
        public const int TXN_DIRECTDEBIT = 15;
        public const int TXN_DIRECTCREDIT = 17;
        public const int TXN_ANTIFRAUD_PAY = 21;
        public const int TXN_ANTIFRAUD_CHECK = 22;

        /* Request types */
        public const string REQUEST_ECHO = "Echo";
        public const string REQUEST_PAYMENT = "Payment";
        public const string REQUEST_PERIODIC = "Periodic";

        /* Periodic types */
        public const int PERIODIC_ONCE = 1;
        public const int PERIODIC_DAY = 2;
        public const int PERIODIC_CALENDAR = 3;
        public const int PERIODIC_TRIGGERED = 4;

        /* Periodic actions */
        public const string ACTION_ADD = "add";
        public const string ACTION_DELETE = "delete";
        public const string ACTION_TRIGGER = "trigger";

        /* Calendar Intervals */
        public const int CAL_WEEKLY = 1;
        public const int CAL_FORTNIGHTLY = 2;
        public const int CAL_MONTHLY = 3;
        public const int CAL_QUARTERLY = 4;
        public const int CAL_HALF_YEARLY = 5;
        public const int CAL_ANNUALLY = 6;

        /* Currencies */
        public const string CURRENCY_DEFAULT = "AUD";

        public const string TIMEOUT = "60";

        /* Errors */
        private const string ERROR_OBJECT_INVALID = "The Gateway Object is invalid";
        private const string ERROR_CURL_ERROR = "CURL failed and reported the following error";
        private const string ERROR_INVALID_AMOUNT = "Parameter Check failure: Amount must be greater than 0";
        private const string ERROR_INVALID_CCNUMBER = "Parameter Check failure: Invalid credit card number";
        private const string ERROR_INVALID_CCEXPIRY = "Parameter Check failure: Invalid credit card expiry date";
        private const string ERROR_INVALID_CC_CVC = "Parameter Check failure: Invalid credit card verification code";
        private const string ERROR_INVALID_DETAILS = "Parameter Check failure: Invalid payment details (non-alphanumeric chars)";
        private const string ERROR_INVALID_TXN_AMT = "Parameter Check failure: Invalid transaction amount";
        private const string ERROR_INVALID_REF_ID = "Parameter Check failure: Invalid transaction reference number";
        private const string ERROR_INVALID_START_DATE = "Parameter Check failure: Invalid start date";
        private const string ERROR_INVALID_REQUEST = "Request failure: Tried to pass Periodic payment through Payment gateway or vice versa";
        private const string ERROR_INVALID_ACCOUNTNUMBER = "Parameter Check failure: Invalid account number";
        private const string ERROR_INVALID_ACCOUNTNAME = "Parameter Check failure: Invalid account name";
        private const string ERROR_INVALID_ACCOUNTBSB = "Parameter Check failure: Invalid BSB";
        private const string ERROR_INVALID_MODE = "Parameter Check failure: Invalid gateway mode";
        private const string ERROR_INVALID_LOGIN = "Parameter Check failure: Invalid merchant account details";
        private const string ERROR_RESPONSE_ERROR = "A general response error was detected";
        private const string ERROR_RESPONSE_INVALID = "A unspecified error was detected in the response content";
        private const string ERROR_XML_PARSE_FAILED = "The response message could not be parsed (invalid XML?)";
        private const string ERROR_RESPONSE_XML_MESSAGE_ERROR = "An unspecified error was found in the response message (missing field?)";
        private const string ERROR_STATUS = "The gateway reported the following status error";
        private const string ERROR_TXN_DECLINED = "Transaction Declined";

        /* Common */
        private string _reference, _error, _url, _merch_id, _merch_pass, _transaction_id, _preauth_id, _request_string, _response_string = "";
        private bool _valid = true;
        private bool _approved = false;
        private int _amount, _transaction_type;

        private Dictionary<string, string> _response = new Dictionary<string, string>();

        /* cc */
        private string _cc_n, _cc_m, _cc_y, _cc_v, _currency = "";

        /* Direct Entry */
        //private string _account_number, _account_bsb, _account_name = "";

        /* Periodic/Triggered */
        private string _request, _start_date = "";
        private int _interval, _number_of_payments, _periodic_type = 0;
        private string _action, _client_id = "";

        /* Support Identifier. re: Richard */
        private string _identifier = "";

        public string Request { get { return this._request_string; } } //Sanitised request string
        public string Response { get { return this._response_string; } }
        public string Identifier { get { return this._identifier; } }
        public string Error { get { return this._error; } }
        public bool Valid { get { return this._valid; } }
        public bool Approved { get { return this._approved; } }
        public decimal Amount { get { return this._amount; } set { if(this._currency != "JPY") { this._amount = (int)(value * 100); } else { this._amount = (int)value; } } }

        public string this[string key] { get { return this._response[key]; } }

        /* Transaction (int mode, merch_id, merch_pass, identifier)
         * 
         * mode is one of this.MODE_*
         * 
         * In standard live/test mode, only Credit/Preauth/Advice/Reverse/Refund transactions will be processed correctly, in preauth live/test mode, only Preauth/Trigger/Delete transactions will be processed. Fraudguard transactions are disabled in this class for now.
         * 
         * merch_id & merch_pass are the merchant's login details for test or live mode respectively. 
         * 
         * identifier is an additional software identifier used by SecurePay support to aid in identifying your transactions if there are any issues. Set it to a description of your payment software and this library's version.
         */
        public XmlTransaction(int mode, string merch_id, string merch_pass, string identifier)
        {
            this._request = REQUEST_PAYMENT;

            switch(mode)
            {
                case MODE_TEST:
                    this._url = URL_TEST;
                    break;

                case MODE_LIVE:
                    this._url = URL_LIVE;
                    break;

                case MODE_PERIODIC_TEST:
                    this._url = URL_PERIODIC_TEST;
                    this._request = REQUEST_PERIODIC;
                    break;

                case MODE_PERIODIC_LIVE:
                    this._url = URL_PERIODIC_LIVE;
                    this._request = REQUEST_PERIODIC;
                    break;

                default:
                    this._valid = false;
                    this._error = ERROR_INVALID_MODE;
                    break;
            }

            this._identifier = identifier;

            if(merch_id.Length == 0 || merch_pass.Length == 0)
            {
                this._valid = false;
                this._error = ERROR_INVALID_LOGIN;
                return;
            }

            this._merch_id = merch_id;
            this._merch_pass = merch_pass;

            return;
        }

        public void reset()
        {
            /* Common */
            this._approved = false;
            this._amount = 0; this._transaction_type = 0; this._transaction_id = ""; this._preauth_id = "";
            this._response = new Dictionary<string, string>();

            return;
        }

        /* bool processCredit (decimal amount, string reference, string cc_n, string cc_m, string cc_y, string cc_v, string currency)
         * 
         * Issues a standard credit-card transaction. 
         * 
         * Returns boolean true if transaction is successful, otherwise false
         */
        public bool processCredit(decimal amount, string reference, string cc_n, string cc_m, string cc_y, string cc_v, string currency)
        {
            this._currency = currency;

            this.Amount = amount;

            this._transaction_type = TXN_STANDARD;
            this._action = ACTION_ADD;
            this._reference = reference;

            this._cc_n = cc_n;
            this._cc_y = cc_y;
            this._cc_m = cc_m;
            this._cc_v = cc_v;

            return this.processTransaction();
        }

        public bool processCredit(decimal amount, string reference, string cc_n, string cc_m, string cc_y, string cc_v)
        {
            return processCredit(amount, reference, cc_n, cc_m, cc_y, cc_v, CURRENCY_DEFAULT);
        }

        public bool processCredit(decimal amount, string reference, string cc_n, string cc_m, string cc_y)
        {
            return processCredit(amount, reference, cc_n, cc_m, cc_y, "", CURRENCY_DEFAULT);
        }

        /* bool processPreauth (decimal amount, string reference, string cc_n, string cc_m, string cc_y, string cc_v, string currency)
         * 
         * Issues a standard credit-card transaction. 
         * 
         * Returns boolean true if transaction is successful, otherwise false
         */
        public bool processPreauth(decimal amount, string reference, string cc_n, string cc_m, string cc_y, string cc_v, string currency)
        {
            this._currency = currency;

            this.Amount = amount;

            this._transaction_type = TXN_PREAUTH;
            this._action = ACTION_ADD;
            this._reference = reference;

            this._cc_n = cc_n;
            this._cc_y = cc_y;
            this._cc_m = cc_m;
            this._cc_v = cc_v;

            return this.processTransaction();
        }

        public bool processPreauth(decimal amount, string reference, string cc_n, string cc_m, string cc_y, string cc_v)
        {
            return processPreauth(amount, reference, cc_n, cc_m, cc_y, cc_v, CURRENCY_DEFAULT);
        }

        public bool processPreauth(decimal amount, string reference, string cc_n, string cc_m, string cc_y)
        {
            return processPreauth(amount, reference, cc_n, cc_m, cc_y, "", CURRENCY_DEFAULT);
        }

        /* bool processAdvice (decimal amount, string reference, string preauth_id)
         * 
         * Completes a previously authorised transaction. amount can be any amount up to the amount originally preauthorised (inclusive). Reference must be the same as in preauthorisation, and the preauth_id should have been collected and saved after a successful Preauth.
         * 
         * An advice transaction can only be successfully submitted once for each preauth_id.
         * 
         * Returns boolean true if transaction is successful, otherwise false
         */
        public bool processAdvice(decimal amount, string reference, string preauth_id)
        {
            this.Amount = amount;

            this._transaction_type = TXN_ADVICE;
            this._action = ACTION_ADD;
            this._reference = reference;
            this._preauth_id = preauth_id;

            return this.processTransaction();
        }

        /* bool processReverse (decimal amount, string reference, string transaction_id)
         * 
         * Refunds the given amount from a non-periodic transaction. Transaction_id and reference need to be the same as in original transaction, and amounts greater than the original transactions total (cumulative) will fail.
         * 
         * Returns boolean true if transaction is successful, otherwise false
         */
        public bool processRefund(decimal amount, string reference, string transaction_id)
        {
            this.Amount = amount;

            this._transaction_type = TXN_REFUND;
            this._action = ACTION_ADD;
            this._reference = reference;
            this._transaction_id = transaction_id;

            return this.processTransaction();
        }

        /* bool processReverse (decimal amount, string reference, string transaction_id)
         * 
         * Reverses a given non-periodic transaction. Amount, transaction_id and reference need to be the same as in original transaction.
         * 
         * Returns boolean true if transaction is successful, otherwise false
         */
        public bool processReverse(decimal amount, string reference, string transaction_id)
        {
            this.Amount = amount;

            this._transaction_type = TXN_REVERSE;
            this._action = ACTION_ADD;
            this._reference = reference;
            this._transaction_id = transaction_id;

            return this.processTransaction();
        }

        /* bool processPeriodic (decimal amount, string client_id, int type, int interval, string start_date, int payments, string cc_n, string cc_m, string cc_y, string cc_v, string currency)
         * 
         * Stores the payment details (cc_n,cc_m,cc_y,cc_v) against the client id in the gateway database for the selected periodic transaction type (type), which depending on type will have any or none of 'interval', 'start date' and 'number of payments'. See the constants at the top of this class for possible values for type & interval, and refer to the integration documentation at http://www.securepay.com.au/resources/Secure_XML_API_Integration_Guide_Periodic_and_Triggered_add_in.pdf
         * 
         * Returns boolean true if transaction is successful, otherwise false
         */
        public bool processPeriodic(decimal amount, string client_id, int type, int interval, string start_date, int payments, string cc_n, string cc_m, string cc_y, string cc_v, string currency)
        {
            this.Amount = amount;

            this._transaction_type = TXN_STANDARD;
            this._action = ACTION_ADD;
            this._client_id = client_id;

            this._cc_n = cc_n;
            this._cc_y = cc_y;
            this._cc_m = cc_m;
            this._cc_v = cc_v;

            this._periodic_type = type;
            this._interval = interval;
            this._start_date = start_date;
            this._number_of_payments = payments;

            return this.processTransaction();
        }

        public bool processPeriodic(decimal amount, string client_id, int type, int interval, string start_date, int payments, string cc_n, string cc_m, string cc_y, string cc_v)
        {
            return processPeriodic(amount, client_id, type, interval, start_date, payments, cc_n, cc_m, cc_y, cc_v, CURRENCY_DEFAULT);
        }

        public bool processPeriodic(decimal amount, string client_id, int type, int interval, string start_date, int payments, string cc_n, string cc_m, string cc_y)
        {
            return processPeriodic(amount, client_id, type, interval, start_date, payments, cc_n, cc_m, cc_y, "", CURRENCY_DEFAULT);
        }

        /* bool processTrigger (decimal amount, string client_id)
         * 
         * Sends a transaction to issue a charge for the given amount against the stored payment details associated with the given client id
         * 
         * Returns boolean true if transaction is successful, otherwise false
         */
        public bool processTrigger(decimal amount, string client_id)
        {
            this._transaction_type = TXN_PERIODIC;
            this._action = ACTION_TRIGGER;
            this._client_id = client_id;

            this.Amount = amount;

            return this.processTransaction();
        }

        /* bool processDelete (string client_id)
         * 
         * Sends a transaction to delete the supplied client_id and associated payment information (and scheduled transactions) from the gateway's database.
         * 
         * Returns boolean true if transaction is successful, otherwise false
         */
        public bool processDelete(string client_id)
        {
            this._transaction_type = TXN_PERIODIC;
            this._action = ACTION_DELETE;
            this._client_id = client_id;

            return this.processTransaction();
        }

        /* bool processTransaction ()
         * 
         * Validates, generates and sends the xml transaction, validates and processes the response.
         * 
         * Returns boolean true if transaction is successful, otherwise false
         */
        private bool processTransaction()
        {
            string request;
            int type = 0;

            if(this.Valid == false)
            {
                this._error = ERROR_OBJECT_INVALID;
                return false;
            }

            type = this._transaction_type;

            if(type == TXN_STANDARD || type == TXN_PREAUTH)
            {
                if(!this.checkCredit())
                {
                    return false;
                }
            }

            if(!this.checkCommon())
            {
                return false;
            }

            request = this.createRequest();
            this._request_string = this.Sanitise(request);
            this._cc_v = ""; this._cc_n = "";

            this._response_string = this.sendRequest(request);

            if(this._response_string == "")
            {
                this._error = ERROR_RESPONSE_ERROR;
                return false;
            }

            return this.processResponse(this._response_string);
        }

        /* bool checkCredit ()
         * 
         * Validates the credit specific variables and fills this._error if necessary
         * 
         * Returns boolean true if valid otherwise false
         */
        private bool checkCredit()
        {
            int month, year, year_delta, this_year;

            if((this._cc_n.Length < 12 || this._cc_n.Length > 19) || Regex.IsMatch(this._cc_n, @"[^0-9]"))
            {
                this._error = ERROR_INVALID_CCNUMBER;
                return false;
            }

            month = Int32.Parse(this._cc_m);
            year = Int32.Parse(this._cc_y);
            this_year = Int32.Parse(DateTime.Today.ToString("yy"));
            year_delta = year - this_year;

            if(!Regex.IsMatch(this._cc_y, @"^[0-9]{2}$") || !Regex.IsMatch(this._cc_m, @"^[0-9]{2}$") || month < 1 || month > 12 || year_delta < 0 || year_delta > 12)
            {
                this._error = ERROR_INVALID_CCEXPIRY;
                return false;
            }

            if(this._cc_v != "" && (Regex.IsMatch(this._cc_v, @"[^0-9]") || this._cc_v.Length > 4 || this._cc_v.Length < 3))
            {
                this._error = ERROR_INVALID_CC_CVC;
                return false;
            }

            return true;
        }

        /* bool checkCommon ()
         * 
         * Validates the common variables and fills this._error if necessary
         * 
         * Returns boolean true if valid otherwise false
         */
        private bool checkCommon()
        {
            if((this._reference == "" && this._client_id == "") ||
                (this._reference != "" && Regex.IsMatch(this._reference, @" '")) || (this._client_id != "" && Regex.IsMatch(this._client_id, @" '"))) //!ANS
            {
                this._error = ERROR_INVALID_REF_ID;
                return false;
            }

            if(this._start_date != "" && !Regex.IsMatch(this._start_date, @"^[0-9]{8}$"))
            {
                this._error = ERROR_INVALID_START_DATE;
                return false;
            }

            if(this._action != ACTION_DELETE && this._amount <= 0)
            {
                this._error = ERROR_INVALID_AMOUNT;
                return false;
            }

            return true;
        }

        /* string createRequest ()
         * 
         * Generates the XML request from instance variables and returns it as a string
         */
        private string createRequest()
        {
            MemoryStream request = new MemoryStream();
            XmlTextWriter xml = new XmlTextWriter(request, new UTF8Encoding(false));
            string result;

            string api, timestamp = "";
            timestamp = this.getTimeStamp();

            if(this._request == REQUEST_PERIODIC)
            {
                api = "spxml-4.2";
            }
            else
            {
                api = "xml-4.2";
            }

            xml.WriteStartDocument(true);

            xml.WriteStartElement("SecurePayMessage");

            xml.WriteStartElement("MessageInfo");
            xml.WriteElementString("messageID", this._reference + timestamp);
            xml.WriteElementString("messageTimestamp", timestamp);
            xml.WriteElementString("timeoutValue", TIMEOUT);
            xml.WriteElementString("apiVersion", api);
            xml.WriteEndElement();

            xml.WriteStartElement("MerchantInfo");
            xml.WriteElementString("merchantID", this._merch_id);
            xml.WriteElementString("password", this._merch_pass);
            xml.WriteEndElement();

            xml.WriteElementString("RequestType", this._request);

            if(this._request == REQUEST_PAYMENT)
            {
                xml.WriteStartElement("Payment");
                xml.WriteStartElement("TxnList");
                xml.WriteAttributeString("count", "1");
                xml.WriteStartElement("Txn");
                xml.WriteElementString("txnType", this._transaction_type.ToString());
                xml.WriteElementString("txnSource", "23"); //Source 23: XML API
                xml.WriteElementString("amount", this._amount.ToString());
                xml.WriteElementString("purchaseOrderNo", this._reference);
            }
            else if(this._request == REQUEST_PERIODIC)
            {
                xml.WriteStartElement("Periodic");
                xml.WriteStartElement("PeriodicList");
                xml.WriteAttributeString("count", "1");
                xml.WriteStartElement("PeriodicItem");
                xml.WriteElementString("actionType", this._action);
                xml.WriteElementString("clientID", this._client_id);
            }

            if(this._transaction_type == TXN_ADVICE)
            {
                xml.WriteElementString("preauthID", this._preauth_id);
            }
            else if(this._transaction_type == TXN_REVERSE || this._transaction_type == TXN_REFUND)
            {
                xml.WriteElementString("txnID", this._transaction_id);
            }

            if(this._currency != CURRENCY_DEFAULT)
            {
                xml.WriteElementString("currency", this._currency);
            }

            if((this._transaction_type == TXN_STANDARD || this._transaction_type == TXN_PREAUTH) && this._action == ACTION_ADD)
            {
                xml.WriteStartElement("CreditCardInfo");
                xml.WriteElementString("cardNumber", this._cc_n);

                xml.WriteElementString("expiryDate", string.Format("{0}/{1}", this._cc_m, this._cc_y));

                if(this._cc_v != "")
                {
                    xml.WriteElementString("cvv", this._cc_v);
                }

                xml.WriteEndElement();
            }

            if(this._request == REQUEST_PERIODIC)
            {
                if(this._action == ACTION_ADD)
                {
                    xml.WriteElementString("amount", this._amount.ToString());

                    xml.WriteElementString("periodicType", this._periodic_type.ToString());

                    if(this._periodic_type != PERIODIC_TRIGGERED)
                    {
                        xml.WriteElementString("startDate", this._start_date);

                        if(this._periodic_type != PERIODIC_ONCE)
                        {
                            xml.WriteElementString("paymentInterval", this._interval.ToString());
                            xml.WriteElementString("numberOfPayments", this._number_of_payments.ToString());
                        }
                    }
                }
                else if(this._action == ACTION_TRIGGER)
                {
                    xml.WriteElementString("amount", this._amount.ToString());
                }
            }

            xml.WriteEndElement();
            xml.WriteEndElement();
            xml.WriteEndElement();

            xml.WriteElementString("identifier", this._identifier);

            xml.WriteEndElement();

            xml.Flush();
            xml.Close();

            result = Encoding.UTF8.GetString(request.ToArray());

            return result;
        }

        /* Sanitise xml request for in-cart display, if required. */
        private string Sanitise(string request)
        {
            if(this._action == ACTION_ADD && (this._transaction_type == TXN_STANDARD || this._transaction_type == TXN_PREAUTH))
            {
                request = request.Replace(this._cc_n, "".PadLeft(this._cc_n.Length, '*'));
                if(this._cc_v != "")
                {
                    request = request.Replace(this._cc_v, "".PadLeft(this._cc_v.Length, '*'));
                }
            }
            request = request.Replace(this._merch_pass, "".PadLeft(this._merch_pass.Length, '*'));

            return request;
        }

        /* string sendRequest (string xml)
         * 
         * Sends the xml request as post data to the gateway, returns the reply.
         * 
         * string xml: gateway request in XML format
         * Returns gateway response in XML format
         */
        private string sendRequest(string xml)
        {
            string result = "";
            byte[] data = UTF8Encoding.ASCII.GetBytes(xml.ToCharArray());
            StreamReader output;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this._url);
            request.Method = "POST";
            request.ContentLength = data.Length;

            using(Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Close();
            }

            WebResponse response = request.GetResponse();

            using(Stream stream = response.GetResponseStream())
            {
                output = new StreamReader(stream);
                result = output.ReadToEnd();
                stream.Close();
            }

            return result;
        }

        /* bool processResponse (string response)
         * 
         * Parses the gateway's response with XmlToArray() and places the important values in the this._response array. Indicates whether the transaction has succeeded or not, and fills this._error with a reason for failure if appropriate.
         * 
         * string response: gateway response in XML format as a string
         * Returns boolean true for successful transaction and successful parse else false
         */
        private bool processResponse(string response)
        {
            XmlToDictionary xml_parser = new XmlToDictionary();
            Dictionary<string, string> result = new Dictionary<string, string>();
            result = xml_parser.Parse(response);

            this._response = new Dictionary<string, string>();

            string apr;
            string path;

            this._approved = false;
            this._response["response"] = response;

            if(this._request == REQUEST_PERIODIC)
            {
                apr = "successful";
                path = "Periodic_PeriodicList_PeriodicItem";
            }
            else // if (this._request == REQUEST_PAYMENT)
            {
                apr = "approved";
                path = "Payment_TxnList_Txn";
            }

            try
            {
                this._response["status"] = result["SecurePayMessage_Status_statusCode"];
                this._response["status_desc"] = result["SecurePayMessage_Status_statusDescription"];
                this._response["approved"] = result.ContainsKey("SecurePayMessage_" + path + "_" + apr) ? result["SecurePayMessage_" + path + "_" + apr] : "No";

                this._response["response_code"] = result.ContainsKey("SecurePayMessage_" + path + "_responseCode") ? result["SecurePayMessage_" + path + "_responseCode"] : this._response["status"];
                this._response["response_text"] = result.ContainsKey("SecurePayMessage_" + path + "_responseText") ? result["SecurePayMessage_" + path + "_responseText"] : this._response["status_desc"];

                if(this._response["approved"].ToUpper() == "YES")
                {
                    this._approved = true;
                }
                else
                {
                    this._approved = false;
                }
            }
            catch(Exception e)
            {
                this._error = ERROR_RESPONSE_INVALID;
                Console.WriteLine("error occured: " + e.Message);
                return false;
            }

            try
            {
                this._response["message_id"] = result["SecurePayMessage_MessageInfo_messageID"];
                this._response["timestamp"] = result["SecurePayMessage_MessageInfo_messageTimestamp"];
                this._response["api"] = result["SecurePayMessage_MessageInfo_apiVersion"];
                this._response["merch_id"] = result["SecurePayMessage_MerchantInfo_merchantID"];

                if(this._request == REQUEST_PAYMENT)
                {
                    this._response["transaction_id"] = result["SecurePayMessage_" + path + "_txnID"];
                    this._response["settlement"] = result["SecurePayMessage_" + path + "_settlementDate"];
                    this._response["type"] = result["SecurePayMessage_" + path + "_txnType"];

                    if(this._transaction_type == TXN_PREAUTH)
                    {
                        this._response["preauth_id"] = result["SecurePayMessage_" + path + "_preauthID"];
                    }
                }

                if(this._transaction_type == TXN_STANDARD || this._transaction_type == TXN_PREAUTH)
                {
                    this._response["cc_pan"] = result["SecurePayMessage_" + path + "_CreditCardInfo_pan"];
                    this._response["cc_expiry"] = result["SecurePayMessage_" + path + "_CreditCardInfo_expiryDate"];
                }

                if(this._transaction_type == TXN_STANDARD || this._transaction_type == TXN_ADVICE || this._transaction_type == TXN_REFUND || this._transaction_type == TXN_REVERSE)
                {
                    this._response["amount"] = result["SecurePayMessage_" + path + "_amount"];
                }
            }
            catch(Exception e)
            {
                if(this._approved)
                {
                    throw e;
                }
            }

            if(this._approved == false)
            {
                this._error = string.Format("{0} ({1}): {2}", ERROR_TXN_DECLINED, this._response["response_code"], this._response["response_text"]);
                return false;
            }

            return true;
        }

        /* string getTimeStamp ()
         * 
         * Returns a timestamp according to the format below as a string
         */
        private string getTimeStamp()
        {
            DateTime now = DateTime.Now;
            string timestamp, tz_string;
            int tz_minutes;

            tz_minutes = Int32.Parse(now.ToString("zz", DateTimeFormatInfo.InvariantInfo)) * 60;

            if(tz_minutes >= 0)
            {
                tz_string = "+" + tz_minutes.ToString();
            }
            else
            {
                tz_string = tz_minutes.ToString();
            }

            /**
                Format: YYYYDDMMHHNNSSKKK000sOOO
                YYYY is a 4-digit year
                DD is a 2-digit zero-padded day of month
                MM is a 2-digit zero-padded month of year (January = 01)
                HH is a 2-digit zero-padded hour of day in 24-hour clock format (midnight =0)
                NN is a 2-digit zero-padded minute of hour
                SS is a 2-digit zero-padded second of minute
                KKK is a 3-digit zero-padded millisecond of second
                000 is a Static 0 characters, as SecurePay does not store nanoseconds
                sOOO is a Time zone offset, where s is + or -, and OOO = minutes, from GMT.
			 	
                i.e. 20092312115518323000+660 as 23/12/2009 at 12:11:55.18323 AEST (DST)
            */
            timestamp = now.ToString("yyyyddMMHHmmss000000", DateTimeFormatInfo.InvariantInfo) + tz_string.ToString();

            return timestamp;
        }
    }

    /* XmlToDictionary
	 * 
	 * Provides XmlToArray.Parse, which converts XML into a dictionary of string key/value pairs.
	 * 
	 * Nested tags are represented in this format:
	 *  object["parent_child1_subchild1"] = "first value"
	 *  object["parent_child2"] = "second value"
	 *  
	 * Tag attributes are ignored, along with any other information in the markup.
	 */
    public class XmlToDictionary
    {
        private Stack<string> Reverse(Stack<string> s)
        {
            Stack<string> new_stack = new Stack<string>();

            foreach(string entry in s)
            {
                new_stack.Push(entry);
            }

            return new_stack;
        }

        private string Collapse(Stack<string> c)
        {
            StringBuilder output = new StringBuilder("");
            int count = 0;

            foreach(string e in c)
            {
                if(count != 0)
                {
                    output.Append(String.Format("_{0}", e));
                }
                else
                {
                    output.Append(e);
                    count++;
                }
            }

            return output.ToString();
        }

        public Dictionary<string, string> Parse(string xml)
        {
            Stack<string> path = new Stack<string>();
            Dictionary<string, string> output = new Dictionary<string, string>();
            string current = null;

            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            reader.WhitespaceHandling = WhitespaceHandling.None;

            try
            {
                while(reader.Read())
                {
                    switch(reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            path.Push(reader.Name);
                            current = this.Collapse(this.Reverse(path));
                            break;

                        case XmlNodeType.EndElement:
                            path.Pop();
                            break;

                        case XmlNodeType.Text:
                            output[current] = reader.Value;
                            break;
                    }
                }
            }
            catch(XmlException e)
            {
                Console.WriteLine("error occured: " + e.Message);
            }
            finally
            {
                if(reader != null)
                {
                    reader.Close();
                }
            }

            return output;
        }
    }
}
