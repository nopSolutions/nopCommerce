using System.IO;
using System.Net;
using System.Text;

namespace Nop.Plugin.Payments.Param
{
    /// <summary>
    /// Summary description for GatewayConnector.
    /// Copyright Web Active Corporation Pty Ltd  - All rights reserved. 1998-2004
    /// This code is for exclusive use with the Param payment gateway
    /// </summary>
    public class GatewayConnector
    {
        string _uri = string.Empty;

        int _timeout = 36000;

        /// <summary>
        /// The Uri of the Param payment gateway
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
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        //public GatewayResponse ProcessRequest(GatewayRequest request)
        //{
        //    var curentRequest = (HttpWebRequest)WebRequest.Create(_uri);
        //    curentRequest.Method = "POST";
        //    curentRequest.Timeout = _timeout;
        //    curentRequest.ContentType = "application/x-www-form-urlencoded";
        //    curentRequest.KeepAlive = false;

        //    var requestBytes = Encoding.ASCII.GetBytes(request.ToXml());
        //    curentRequest.ContentLength = requestBytes.Length;

        //    // Send the data out over the wire
        //    var requestStream = curentRequest.GetRequestStream();
        //    requestStream.Write(requestBytes, 0, requestBytes.Length);
        //    requestStream.Close();

        //    var response = (HttpWebResponse)curentRequest.GetResponse();

        //    using (var sr = new StreamReader(response.GetResponseStream(), Encoding.ASCII))
        //    {
        //        return new GatewayResponse(sr.ReadToEnd());
        //    }
        //}
    }
}
