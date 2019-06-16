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
using System.Globalization;
using System.Xml;
using System.Net;

namespace Uol.PagSeguro
{
    internal class QueryStringBuilder
    {
        private const char Separator = '&';
        private const char Equal = '=';
        private StringBuilder builder;

        public QueryStringBuilder()
        {
            builder = new StringBuilder();
        }

        public QueryStringBuilder(string queryString)
        {
            if (queryString == null)
                throw new ArgumentNullException("queryString");

            builder = new StringBuilder(queryString);
        }

        private void AppendCore(string parameterName, string value)
        {
            if (builder.Length > 0)
            {
                builder.Append(QueryStringBuilder.Separator);
            }
            builder.Append(WebUtility.UrlEncode(parameterName));
            builder.Append(QueryStringBuilder.Equal);
            builder.Append(WebUtility.UrlEncode(value));
        }

        public QueryStringBuilder Append(string parameterName, string value)
        {
            if (String.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException("parameterName");
            if (String.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            AppendCore(parameterName, value);

            return this;
        }

        public QueryStringBuilder Append(string parameterName, int value)
        {
            if (String.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException("parameterName");

            AppendCore(parameterName, value.ToString(CultureInfo.InvariantCulture));

            return this;
        }

        public QueryStringBuilder Append(string parameterName, long value)
        {
            if (String.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException("parameterName");

            AppendCore(parameterName, value.ToString(CultureInfo.InvariantCulture));

            return this;
        }

        public QueryStringBuilder Append(string parameterName, decimal value)
        {
            if (String.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException("parameterName");

            AppendCore(parameterName, value.ToString(CultureInfo.InvariantCulture));

            return this;
        }

        public QueryStringBuilder Append(string parameterName, DateTime value)
        {
            if (String.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException("parameterName");
            AppendCore(parameterName, XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc));

            return this;
        }

        public override string ToString()
        {
            return this.builder.ToString();
        }
    }
}
