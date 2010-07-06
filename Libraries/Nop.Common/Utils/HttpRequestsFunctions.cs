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
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using NopSolutions.NopCommerce.Common.Utils.Html;
using System.Net;

namespace NopSolutions.NopCommerce.Common.Utils
{
    public static class HttpRequestsFunctions
    {
        public static string HttpPost(string uri, string parameters)
        {
            // parameters: name1=value1&name2=value2	
            WebRequest webRequest = WebRequest.Create(uri);

            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(parameters);
            Stream os = null;
            try
            { // send the Post
                webRequest.ContentLength = bytes.Length;   //Count bytes to send
                os = webRequest.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);         //Send it
            }
            catch (WebException ex)
            {
                throw new NopException("HttpPost: Request error", ex.InnerException);
            }
            finally
            {
                if (os != null)
                {
                    os.Close();
                }
            }

            try
            { // get the response
                WebResponse webResponse = webRequest.GetResponse();
                if (webResponse == null)
                { return null; }
                StreamReader sr = new StreamReader(webResponse.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }
            catch (WebException ex)
            {
                throw new NopException("HttpPost: Response error", ex.InnerException);
            }
            return null;
        } // end HttpPost 

        private static Stream getResponseStream(WebRequest request)
        {
            //grab the respons stream
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream ResponseStream = response.GetResponseStream();


            //create the response buffer
            byte[] ResponseBuffer = new byte[response.ContentLength];
            int BytesLeft = ResponseBuffer.Length;
            int TotalBytesRead = 0;
            bool EOF = false;

            //loop while not EOF
            while (!EOF)
            {
                //get the next chunk and calc the remaining bytes
                int BytesRead = ResponseStream.Read(ResponseBuffer, TotalBytesRead, BytesLeft);
                TotalBytesRead += BytesRead;
                BytesLeft -= BytesRead;

                //has EOF been reached
                EOF = (BytesLeft == 0);
            }



            ResponseStream.Close();

            //create a memory stream and pass in the respones buffer
            ResponseStream = new MemoryStream(ResponseBuffer);
            return ResponseStream;

        }




        private static XmlDocument GetResponseXml(WebRequest request)
        {
            //load the stream into an xml document
            XmlDocument ResponseDocument = new XmlDocument();
            ResponseDocument.Load(getResponseStream(request));
            return ResponseDocument;

        }


        public static string PostRetString(string url)
        {
            Stream stream = MakeRequestGet(url);
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static string PostRetString(string url, XmlDocument document)
        {
            WebRequest request = MakeRequestPost(url, document);
            Stream stream = getResponseStream(request);
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }


        private static Stream MakeRequestGet(string url)
        {
            WebRequest Request = WebRequest.Create(url);
            Request.Method = "GET";
            Request.ContentType = "application/x-www-form-urlencoded";

            // If required by the server, set the credentials.
            Request.Credentials = CredentialCache.DefaultCredentials;

            // Request.ContentLength = 
            HttpWebResponse response = (HttpWebResponse)Request.GetResponse();
            Stream RequestStream = response.GetResponseStream();
            return RequestStream;
        }

        private static WebRequest MakeRequestPost(string url, XmlDocument document)
        {
            //convert the document to stream
            MemoryStream Stream = new MemoryStream();
            XmlWriter Writer = XmlWriter.Create(Stream);
            document.WriteContentTo(Writer);

            //reset the stream position
            Stream.Position = 0;

            //create the request and set the content type and length
            WebRequest Request = WebRequest.Create(url);
            Request.Method = "POST";
            Request.ContentType = "text/xml";
            Request.ContentLength = Stream.Length;
            
            //get the request stream and post the xml
            Stream RequestStream = Request.GetRequestStream();
            RequestStream.Write(Stream.GetBuffer(), 0, (int)Stream.Length);
            return Request;
        }

        public static XmlDocument PostRetXml(string url, XmlDocument document)
        {
            WebRequest Request = MakeRequestPost(url, document);
            return GetResponseXml(Request);

        }


    }
}
