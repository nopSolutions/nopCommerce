// Copyright [2011] [PagSeguro Internet Ltda.]
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Text;
using System.Net;
using System.Xml;
using Uol.PagSeguro.Serialization;
using System.Globalization;

namespace Uol.PagSeguro
{
    /// <summary>
    /// Encapsulates web service calls regarding PagSeguro notifications
    /// </summary>
    public static class NotificationService
    {
        /// <summary>
        /// Returns a transaction from a notification code
        /// </summary>
        /// <param name="credentials">PagSeguro credentials</param>
        /// <param name="notificationCode">Transaction notification code</param>
        /// <returns><c cref="T:Uol.PagSeguro.Transaction">Transaction</c></returns>
        public static Transaction CheckTransaction(Credentials credentials, string notificationCode)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");
            if (String.IsNullOrEmpty(notificationCode))
                throw new ArgumentNullException("notificationCode");

            var uriBuilder = new UriBuilder(PagSeguroConfiguration.GetNotificationUri(credentials.IsSandbox));
            var pathBuilder = new StringBuilder(uriBuilder.Path);
            pathBuilder.Append('/');
            pathBuilder.Append(WebUtility.UrlEncode(notificationCode));
            pathBuilder.Append('/');
            uriBuilder.Path = pathBuilder.ToString();
            uriBuilder.Query = ServiceHelper.EncodeCredentialsAsQueryString(credentials);

            WebRequest request = WebRequest.Create(uriBuilder.Uri);
            request.Method = ServiceHelper.GET_METHOD;
            request.Timeout = PagSeguroConfiguration.RequestTimeout;

            PagSeguroTrace.Info(string.Format(CultureInfo.InvariantCulture, "NotificationService.CheckTransaction(notificationCode={0}) - begin", notificationCode));

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                    {
                        Transaction transaction = new Transaction();
                        TransactionSerializer.Read(reader, transaction);

                        PagSeguroTrace.Info(String.Format(CultureInfo.InvariantCulture, "NotificationService.CheckTransaction(notificationCode={0}) - end {1}", notificationCode, transaction));
                        return transaction;
                    }
                }
            }
            catch (WebException exception)
            {
                PagSeguroServiceException pse = ServiceHelper.CreatePagSeguroServiceException((HttpWebResponse)exception.Response);
                PagSeguroTrace.Error(
                    string.Format(CultureInfo.InvariantCulture, "NotificationService.CheckTransaction(notificationCode={0}) - error {1}", notificationCode, pse));
                throw pse;
            }
        }
    }
}
