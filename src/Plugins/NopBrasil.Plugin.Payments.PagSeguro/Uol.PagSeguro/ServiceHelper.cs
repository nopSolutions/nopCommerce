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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Uol.PagSeguro.Serialization;
using System.Net;

namespace Uol.PagSeguro
{
    internal static class ServiceHelper
    {
        internal const string GET_METHOD = "GET";
        internal const string POST_METHOD = "POST";
        internal const string FORM_URL_ENCODED = "application/x-www-form-urlencoded";
        internal const string XML_ENCODED = "application/xml";

        internal static PagSeguroServiceException CreatePagSeguroServiceException(HttpWebResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
                throw new ArgumentException("response.StatusCode must be different than HttpStatusCode.OK", "response");

            using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        var errors = new List<PagSeguroServiceError>();
                        try
                        {
                            PagSeguroErrorsSerializer.Read(reader, errors);
                        }
                        catch (XmlException e)
                        {
                            return new PagSeguroServiceException(response.StatusCode, e);
                        }

                        return new PagSeguroServiceException(response.StatusCode, errors);

                    default:
                        return new PagSeguroServiceException(response.StatusCode);
                }
            }
        }

        internal static string EncodeCredentialsAsQueryString(Credentials credentials)
        {
            var builder = new QueryStringBuilder();
            foreach (var nv in credentials.Attributes)
            {
                builder.Append(nv.Name, nv.Value);
            }
            return builder.ToString();
        }
    }
}
