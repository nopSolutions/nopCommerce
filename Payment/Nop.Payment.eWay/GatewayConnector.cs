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

using System.IO;
using System.Net;
using System.Text;

namespace NopSolutions.NopCommerce.Payment.Methods.eWay
{
    /// <summary>
    /// Summary description for GatewayConnector.
    /// Copyright Web Active Corporation Pty Ltd  - All rights reserved. 1998-2004
    /// This code is for exclusive use with the eWAY payment gateway
    /// </summary>
    public class GatewayConnector
    {
        string _uri = string.Empty;

        int _timeout = 36000;

        /// <summary>
        /// GatewayConnector ctor
        /// </summary>
        public GatewayConnector()
        {

        }

        /// <summary>
        /// The Uri of the Eway payment gateway
        /// </summary>
        public string Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }

        /// <summary>
        /// The connection timeout
        /// </summary>
        public int ConnectionTimeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// Do the post to the gateway and retrieve the response
        /// </summary>
        /// <param name="Request">Request</param>
        /// <returns>Response</returns>
        public GatewayResponse ProcessRequest(GatewayRequest Request)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(_uri);
            request.Method = "POST";
            request.Timeout = _timeout;
            request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = false;

            byte[] requestBytes = Encoding.ASCII.GetBytes(Request.ToXml());
            request.ContentLength = requestBytes.Length;

            // Send the data out over the wire
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.ASCII))
            {
                string _serverXml = sr.ReadToEnd();
                return new GatewayResponse(_serverXml);
            }
        }
    }
}