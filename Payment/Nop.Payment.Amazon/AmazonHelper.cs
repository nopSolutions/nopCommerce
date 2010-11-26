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
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.Payment.Methods.Amazon
{
    public class AmazonHelper
    {
        public static readonly String SIGNATURE_KEYNAME = "signature";
        public static readonly String SIGNATURE_METHOD_KEYNAME = "signatureMethod";
        public static readonly String SIGNATURE_VERSION_KEYNAME = "signatureVersion";
        public static readonly String SIGNATURE_VERSION_1 = "1";
        public static readonly String SIGNATURE_VERSION_2 = "2";
        public static readonly String HMAC_SHA1_ALGORITHM = "HmacSHA1";
        public static readonly String HMAC_SHA256_ALGORITHM = "HmacSHA256";
        public static readonly String RSA_SHA1_ALGORITHM = "SHA1withRSA";
        public static readonly String CERTIFICATE_URL_KEYNAME = "certificateUrl";
        public static readonly String EMPTY_STRING = String.Empty;

        // Constants used when constructing the string to sign for v2
        public static readonly String AppName = "ASP";
        public static readonly String NewLine = "\n";
        public static readonly String EmptyUriPath = "/";
        public static String equals = "=";
        public static readonly String And = "&";
        public static readonly String UTF_8_Encoding = "UTF-8";

        /**
        * Calculate String to Sign for SignatureVersion 1
        * @param parameters request parameters
        * @return String to Sign
        */
        public static string SignParameters(NameValueCollection parameters, String key, 
            String HttpMethod, String Host, String RequestURI) //throws Exception
        {
            String signatureVersion = parameters[SIGNATURE_VERSION_KEYNAME];
            String algorithm = HMAC_SHA1_ALGORITHM;
            String stringToSign = null;
            if (signatureVersion == null && String.Compare(AppName, "FPS", true) != 0)
            {
                stringToSign = CalculateSignV1(parameters);
            }
            else if (String.Compare("1", signatureVersion, true) == 0)
            {
                stringToSign = CalculateSignV1(parameters);
            }
            else if (String.Compare("2", signatureVersion, true) == 0)
            {
                algorithm = parameters[SIGNATURE_METHOD_KEYNAME];
                stringToSign = CalculateSignV2(parameters, HttpMethod, Host, RequestURI);
            }
            else
            {
                throw new NopException("Invalid Signature Version specified");
            }
            return Sign(stringToSign, key, algorithm);
        }

        /**
	    * Calculate String to Sign for SignatureVersion 1
	    * @param parameters request parameters
	    * @return String to Sign
	    */
        private static string CalculateSignV1(NameValueCollection parameters)
        {
            StringBuilder data = new StringBuilder();
            IDictionary<String, String> sorted = new SortedDictionary<String, String>(StringComparer.OrdinalIgnoreCase);
            foreach(string paramKey in parameters.AllKeys)
            {
                sorted.Add(paramKey, parameters[paramKey]);
            }
            foreach (KeyValuePair<String, String> pair in sorted)
            {
                if (pair.Value != null)
                {
                    if (String.Compare(pair.Key, SIGNATURE_KEYNAME, true) == 0) continue;
                    data.Append(pair.Key);
                    data.Append(pair.Value);
                }
            }
            return data.ToString();
        }

        /**
    	 * Calculate String to Sign for SignatureVersion 2
	     * @param parameters
    	 * @param httpMethod - POST or GET
	     * @param hostHeader - Service end point
    	 * @param requestURI - Path
	     * @return
    	 */
        private static string CalculateSignV2(NameValueCollection parameters, String httpMethod, String hostHeader, String requestURI)// throws SignatureException
        {
            StringBuilder stringToSign = new StringBuilder();
            if (httpMethod == null) throw new Exception("HttpMethod cannot be null");
            stringToSign.Append(httpMethod);
            stringToSign.Append(NewLine);

            // The host header - must eventually convert to lower case
            // Host header should not be null, but in Http 1.0, it can be, in that
            // case just append empty string ""
            if (hostHeader == null)
                stringToSign.Append("");
            else
                stringToSign.Append(hostHeader.ToLower());
            stringToSign.Append(NewLine);

            if (requestURI == null || requestURI.Length == 0)
                stringToSign.Append(EmptyUriPath);
            else
                stringToSign.Append(UrlEncode(requestURI, true));
            stringToSign.Append(NewLine);

            IDictionary<String, String> sortedParamMap = new SortedDictionary<String, String>(StringComparer.Ordinal);
            foreach(string paramKey in parameters.AllKeys)
            {
                sortedParamMap.Add(paramKey, parameters[paramKey]);
            }
            foreach (String key in sortedParamMap.Keys)
            {
                if (String.Compare(key, SIGNATURE_KEYNAME, true) == 0) continue;
                stringToSign.Append(UrlEncode(key, false));
                stringToSign.Append(equals);
                stringToSign.Append(UrlEncode(sortedParamMap[key], false));
                stringToSign.Append(And);
            }

            String result = stringToSign.ToString();
            return result.Remove(result.Length - 1);
        }

        public static String UrlEncode(String data, bool path)
        {
            StringBuilder encoded = new StringBuilder();
            String unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~" + (path ? "/" : "");

            foreach (char symbol in System.Text.Encoding.UTF8.GetBytes(data))
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    encoded.Append(symbol);
                }
                else
                {
                    encoded.Append("%" + String.Format("{0:X2}", (int)symbol));
                }
            }

            return encoded.ToString();

        }

        /**
         * Computes RFC 2104-compliant HMAC signature.
         */
        public static String Sign(String data, String key, String signatureMethod)// throws SignatureException
	    {
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                HMAC Hmac = HMAC.Create(signatureMethod);
                Hmac.Key = encoding.GetBytes(key);
                Hmac.Initialize();
                CryptoStream cs = new CryptoStream(Stream.Null, Hmac, CryptoStreamMode.Write);
                cs.Write(encoding.GetBytes(data), 0, encoding.GetBytes(data).Length);
                cs.Close();
                byte[] rawResult = Hmac.Hash;
                String sig = Convert.ToBase64String(rawResult, 0, rawResult.Length);
                return sig;
            }
            catch (Exception e)
            {
                throw new NopException("Failed to generate signature: " + e.Message);
            }
	    }

            /**
         * Validates the request by checking the integrity of its parameters.
         * @param parameters - all the http parameters sent in IPNs or return urls. 
         * @param urlEndPoint should be the url which recieved this request. 
         * @param httpMethod should be either POST (IPNs) or GET (returnUrl redirections)
         */
        public static Boolean ValidateRequest(NameValueCollection parameters,
               String urlEndPoint, String httpMethod) 
        {
            String signatureVersion = null;
            //This is present only in case of signature version 2. If this is not present we assume this is signature version 1.
            try
            {
                signatureVersion = parameters[SIGNATURE_VERSION_KEYNAME];
            }
            catch (KeyNotFoundException)
            {
                signatureVersion = null;
            }

            if(SIGNATURE_VERSION_2.Equals(signatureVersion))
                return ValidateSignatureV2(parameters, urlEndPoint, httpMethod);
            else
                return ValidateSignatureV1(parameters);
         }

        /**
          * Verifies the signature using PKI.
          */
        private static  Boolean ValidateSignatureV2(NameValueCollection parameters,
            String urlEndPoint, String httpMethod)
        {
            //1. input validation.
            String signature = parameters[SIGNATURE_KEYNAME];
            if (signature == null)
            {
                throw new Exception("'signature' is missing from the parameters.");
            }

            String signatureMethod = parameters[SIGNATURE_METHOD_KEYNAME];
            if (signatureMethod == null)
            {
                throw new Exception("'signatureMethod' is missing from the parameters.");
            }

            String signatureAlgorithm = GetSignatureAlgorithm(signatureMethod);
            if (signatureAlgorithm == null)
            {
                throw new Exception("'signatureMethod' present in parameters is invalid. " +
                        "Valid signatureMethods are : 'RSA-SHA1'");
            }

            String certificateUrl = parameters[CERTIFICATE_URL_KEYNAME];
            if (certificateUrl == null)
            {
                throw new Exception("'certificateUrl' is missing from the parameters.");
            }

            String certificate = GetPublicKeyCertificateAsString(certificateUrl);
            if (certificate == null)
            {
                throw new Exception("public key certificate could not fetched from url: " + certificateUrl);
            }

            //2. calculating the string to sign
            String stringToSign = EMPTY_STRING;
            try
            {
                Uri uri = new Uri(urlEndPoint);
                String hostHeader = getHostHeader(uri);
                String requestURI = GetRequestURI(uri);
                stringToSign = CalculateSignV2(parameters, httpMethod, hostHeader, requestURI);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            //3. verify signature 
            try
            {
                byte[] signatureBytes = Base64DecodeToBytes(signature);
                X509Certificate2 x509Cert = new X509Certificate2(StrToByteArray(certificate));
                RSACryptoServiceProvider RSA = (RSACryptoServiceProvider)x509Cert.PublicKey.Key;
                return RSA.VerifyData(StrToByteArray(stringToSign), new SHA1Managed(), signatureBytes);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static bool ValidateSignatureV1(NameValueCollection parameters)
        {
            String stringToSign = CalculateSignV1(parameters);
            String signature = parameters[SIGNATURE_KEYNAME];
            String sig;
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                HMAC Hmac = HMAC.Create("HmacSHA1");
                Hmac.Key = encoding.GetBytes(SimplePaySettings.SecretKey);
                Hmac.Initialize();
                CryptoStream cs = new CryptoStream(Stream.Null, Hmac, CryptoStreamMode.Write);
                cs.Write(encoding.GetBytes(stringToSign), 0, encoding.GetBytes(stringToSign).Length);
                cs.Close();
                byte[] rawResult = Hmac.Hash;
                sig = Convert.ToBase64String(rawResult, 0, rawResult.Length);

            }
            catch (Exception e)
            {
                throw new Exception("Failed to generate HMAC : " + e.Message);
            }
            return sig.Equals(signature);
        }


        public static string V2UrlEncode(String data, bool path)
        {
            StringBuilder encoded = new StringBuilder();
            String unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~" + (path ? "/" : "");

            foreach (char symbol in System.Text.Encoding.UTF8.GetBytes(data))
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    encoded.Append(symbol);
                }
                else
                {
                    encoded.Append("%" + String.Format("{0:X2}", (int)symbol));
                }
            }
            return encoded.ToString();
        }



        public static string UrlDecode(String value)
        {
            return HttpUtility.UrlDecode(value, Encoding.UTF8);
        }

        private static string getHostHeader(Uri uri)
        {
            int port = uri.Port;
            if (port != -1)
            {
                if(uri.Scheme.Equals(Uri.UriSchemeHttps) && port ==443
                    || uri.Scheme.Equals(Uri.UriSchemeHttp) && port == 80)
                    port = -1;
            }
            return uri.Host.ToLower() + (port != -1 ? ":" + port : "");
        }

        private static string GetRequestURI(Uri uri)
        {
            String requestURI = uri.LocalPath;
            if (requestURI == null || requestURI.Equals(EMPTY_STRING)) {
                requestURI = "/";
            } else {
                requestURI = UrlDecode(requestURI);
            }
            return requestURI;
        }

        private static string GetSignatureAlgorithm(string signatureMethod)
        {
            if ("RSA-SHA1".Equals(signatureMethod)) {
                return RSA_SHA1_ALGORITHM;
            }
            return null;
        }

        private static string GetPublicKeyCertificateAsString(string certificateUrl)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(certificateUrl);
            request.AllowAutoRedirect = false;
            HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            String data = read.ReadToEnd();
            return data;
        }

        /// <summary>
        /// Base64 decode a string
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Data</returns>
        public static byte[] Base64DecodeToBytes(string data)
        {
            return Convert.FromBase64String(data);
        }

        /// <summary>
        /// Convert a string to a byte array
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Byte array</returns>
        public static byte[] StrToByteArray(string str)
        {
            return new UTF8Encoding().GetBytes(str);
        }

    }
}
