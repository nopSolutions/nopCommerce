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
    /// Encapsulates web service calls to search for PagSeguro transactions
    /// </summary>
    public static class TransactionSearchService
    {
        private const string INITIAL_DATE_PARAMETER_NAME = "initialDate";
        private const string FINAL_DATE_PARAMETER_NAME = "finalDate";
        private const string PAGE_NUMBER_PARAMETER_NAME = "page";
        private const string MAX_PAGE_RESULTS_PARAMETER_NAME = "maxPageResults";

        /// <summary>
        /// Finds a transaction with a matching transaction code
        /// </summary>
        /// <param name="credentials">PagSeguro credentials</param>
        /// <param name="transactionCode">Transaction code</param>
        /// <returns cref="T:Uol.PagSeguro.Transaction"><c>Transaction</c></returns>
        public static Transaction SearchByCode(Credentials credentials, string transactionCode)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");
            if (transactionCode == null)
                throw new ArgumentNullException("transactionCode");

            PagSeguroTrace.Info(string.Format(CultureInfo.InvariantCulture, "TransactionSearchService.SearchByCode(transactionCode={0}) - begin", transactionCode));

            var uriBuilder = new UriBuilder(PagSeguroConfiguration.GetSearchUri(credentials.IsSandbox));
            var pathBuilder = new StringBuilder(uriBuilder.Path);

            pathBuilder.Append('/').Append(WebUtility.UrlEncode(transactionCode));
            uriBuilder.Path = pathBuilder.ToString();
            uriBuilder.Query = ServiceHelper.EncodeCredentialsAsQueryString(credentials);

            WebRequest request = WebRequest.Create(uriBuilder.Uri);
            request.Method = ServiceHelper.GET_METHOD;
            request.Timeout = PagSeguroConfiguration.RequestTimeout;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                    {
                        Transaction transaction = new Transaction();
                        TransactionSerializer.Read(reader, transaction);
                        PagSeguroTrace.Info(String.Format(CultureInfo.InvariantCulture, "TransactionSearchService.SearchByCode(transactionCode={0}) - end {1}", transactionCode, transaction));
                        return transaction;
                    }
                }
            }
            catch (WebException exception)
            {
                PagSeguroServiceException pse = ServiceHelper.CreatePagSeguroServiceException((HttpWebResponse)exception.Response);
                PagSeguroTrace.Error(String.Format(CultureInfo.InvariantCulture, "TransactionSearchService.SearchByCode(transactionCode={0}) - error {1}", transactionCode, pse));
                throw pse;
            }
        }

        public static TransactionSearchResult SearchByReference(Credentials credentials, string referenceCode)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");
            if (referenceCode == null)
                throw new ArgumentNullException("referenceCode");

            PagSeguroTrace.Info(string.Format(CultureInfo.InvariantCulture, "TransactionSearchService.SearchByReference(referenceCode={0}) - begin", referenceCode));

            UriBuilder uriBuilder = new UriBuilder(PagSeguroConfiguration.GetSearchUri(credentials.IsSandbox));
            StringBuilder pathBuilder = new StringBuilder(uriBuilder.Path);
            uriBuilder.Path = pathBuilder.ToString();
            uriBuilder.Query = $"{ServiceHelper.EncodeCredentialsAsQueryString(credentials)}&reference={WebUtility.UrlEncode(referenceCode)}";

            WebRequest request = WebRequest.Create(uriBuilder.Uri);
            request.Method = ServiceHelper.GET_METHOD;
            request.Timeout = PagSeguroConfiguration.RequestTimeout;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                    {
                        TransactionSearchResult result = new TransactionSearchResult();
                        TransactionSearchResultSerializer.Read(reader, result);
                        PagSeguroTrace.Info(String.Format(CultureInfo.InvariantCulture, "TransactionSearchService.SearchByReference(referenceCode={0}) - end", referenceCode));
                        return result;
                    }
                }
            }
            catch (WebException exception)
            {
                PagSeguroServiceException pse = ServiceHelper.CreatePagSeguroServiceException((HttpWebResponse)exception.Response);
                PagSeguroTrace.Error(String.Format(CultureInfo.InvariantCulture, "TransactionSearchService.SearchByReference(referenceCode={0}) - error {1}", referenceCode, pse));
                throw pse;
            }
        }

        /// <summary>
        /// Search transactions associated with this set of credentials within a date range
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="initialDate"></param>
        /// <returns></returns>
        public static TransactionSearchResult SearchByDate(Credentials credentials, DateTime initialDate)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");

            return SearchByDateCore(credentials, initialDate, DateTime.MaxValue, 0, 0);
        }

        /// <summary>
        /// Search transactions associated with this set of credentials within a date range
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="initialDate"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public static TransactionSearchResult SearchByDate(Credentials credentials, DateTime initialDate, int pageNumber)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "pageNumber must be greater than zero");

            return SearchByDateCore(credentials, initialDate, DateTime.MaxValue, pageNumber, 0);
        }

        /// <summary>
        /// Search transactions associated with this set of credentials within a date range
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="initialDate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="resultsPerPage"></param>
        /// <returns></returns>
        public static TransactionSearchResult SearchByDate(Credentials credentials, DateTime initialDate, int pageNumber, int resultsPerPage)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "pageNumber must be greater than zero");
            if (resultsPerPage <= 0)
                throw new ArgumentOutOfRangeException("resultsPerPage", resultsPerPage, "resultsPerPage must be greater than zero");

            return SearchByDateCore(credentials, initialDate, DateTime.MaxValue, pageNumber, resultsPerPage);
        }

        /// <summary>
        /// Search transactions associated with this set of credentials within a date range
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="initialDate"></param>
        /// <param name="finalDate"></param>
        /// <returns></returns>
        public static TransactionSearchResult SearchByDate(Credentials credentials, DateTime initialDate, DateTime finalDate)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");
            if (DateTime.Compare(initialDate, finalDate) > 0)
                throw new ArgumentException("initialDate must be before finalDate", "initialDate");

            return SearchByDateCore(credentials, initialDate, finalDate, 0, 0);
        }

        /// <summary>
        /// Search transactions associated with this set of credentials within a date range
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="initialDate"></param>
        /// <param name="finalDate"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public static TransactionSearchResult SearchByDate(Credentials credentials, DateTime initialDate, DateTime finalDate, int pageNumber)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");
            if (DateTime.Compare(initialDate, finalDate) > 0)
                throw new ArgumentException("initialDate must be before finalDate", "initialDate");
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "pageNumber must be greater than zero");

            return SearchByDateCore(credentials, initialDate, finalDate, pageNumber, 0);
        }

        /// <summary>
        /// Search transactions associated with this set of credentials within a date range
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="initialDate"></param>
        /// <param name="finalDate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="resultsPerPage"></param>
        /// <returns></returns>
        public static TransactionSearchResult SearchByDate(Credentials credentials, DateTime initialDate, DateTime finalDate, int pageNumber, int resultsPerPage)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");
            if (DateTime.Compare(initialDate, finalDate) > 0)
                throw new ArgumentException("initialDate must be before finalDate", "initialDate");
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "pageNumber must be greater than zero");
            if (resultsPerPage <= 0)
                throw new ArgumentOutOfRangeException("resultsPerPage", resultsPerPage, "resultsPerPage must be greater than zero");

            return SearchByDateCore(credentials, initialDate, finalDate, pageNumber, resultsPerPage);
        }

        /// <summary>
        /// Common implmentation of all SearchByDate methods
        /// </summary>
        /// <param name="credentials">PagSeguro credentials. Required.</param>
        /// <param name="initialDate"></param>
        /// <param name="finalDate">End of date range. Use DateTime.MaxValue to search without an upper boundary.</param>
        /// <param name="pageNumber">Page number, starting with 1. If passed as 0, it will call the web service to get the default page, also page number 1.</param>
        /// <param name="resultsPerPage">Results per page, optional.</param>
        /// <returns></returns>
        private static TransactionSearchResult SearchByDateCore(Credentials credentials, DateTime initialDate, DateTime finalDate, int pageNumber, int resultsPerPage)
        {
            UriBuilder uriBuilder = new UriBuilder(PagSeguroConfiguration.GetSearchUri(credentials.IsSandbox));

            QueryStringBuilder query = new QueryStringBuilder(ServiceHelper.EncodeCredentialsAsQueryString(credentials));

            query.Append(INITIAL_DATE_PARAMETER_NAME, initialDate);

            if (finalDate < DateTime.MaxValue)
            {
                query.Append(FINAL_DATE_PARAMETER_NAME, finalDate);
            }

            if (pageNumber > 0)
            {
                query.Append(PAGE_NUMBER_PARAMETER_NAME, pageNumber);
            }

            if (resultsPerPage > 0)
            {
                query.Append(MAX_PAGE_RESULTS_PARAMETER_NAME, resultsPerPage);
            }

            uriBuilder.Query = query.ToString();

            PagSeguroTrace.Info(String.Format(CultureInfo.InvariantCulture, "TransactionSearchService.SearchByDate(initialDate={0}, finalDate={1}) - begin", initialDate, finalDate));

            WebRequest request = WebRequest.Create(uriBuilder.Uri);
            request.Method = ServiceHelper.GET_METHOD;
            request.Timeout = PagSeguroConfiguration.RequestTimeout;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                    {
                        TransactionSearchResult result = new TransactionSearchResult();
                        TransactionSearchResultSerializer.Read(reader, result);
                        PagSeguroTrace.Info(String.Format(CultureInfo.InvariantCulture, "TransactionSearchService.SearchByDate(initialDate={0}, finalDate={1}) - end {2}", initialDate, finalDate, result));
                        return result;
                    }
                }
            }
            catch (WebException exception)
            {
                PagSeguroServiceException pse = ServiceHelper.CreatePagSeguroServiceException((HttpWebResponse)exception.Response);
                PagSeguroTrace.Error(String.Format(CultureInfo.InvariantCulture, "TransactionSearchService.SearchByDate(initialDate={0}, finalDate={1}) - error {2}", initialDate, finalDate, pse));
                throw pse;
            }
        }
    }
}