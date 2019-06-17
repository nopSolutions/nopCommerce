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
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Net;
using System.Globalization;
using System.Security.Permissions;

namespace Uol.PagSeguro
{
    /// <summary>
    /// Encapsulates a problem that occurred calling a PagSeguro web service
    /// </summary>
    [Serializable]
    public sealed class PagSeguroServiceException : Exception
    {
        private const string CR_LF = "\n";
        private const string HTTP_STATUS_CODE_FIELD = "HttpStatusCode";
        private const string ERRORS_FIELD = "Errors";
        private List<PagSeguroServiceError> _errors;

        /// <summary>
        /// Initializes a new instance of the PagSeguroServiceException class
        /// </summary>
        public PagSeguroServiceException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PagSeguroServiceException class
        /// </summary>
        public PagSeguroServiceException(Exception exception)
            : base(exception.Message, exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PagSeguroServiceException class
        /// </summary>
        /// <param name="statusCode"></param>
        public PagSeguroServiceException(HttpStatusCode statusCode) :
            base(String.Format(CultureInfo.InvariantCulture, "HttpStatusCode: {0}", statusCode))
        {
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the PagSeguroServiceException class
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="errors"></param>
        public PagSeguroServiceException(HttpStatusCode statusCode, IEnumerable<PagSeguroServiceError> errors) :
            base(String.Format(CultureInfo.InvariantCulture, "HttpStatusCode: {0}", statusCode))
        {
            if (errors == null)
                throw new ArgumentNullException("errors");
            this._errors = new List<PagSeguroServiceError>(errors);
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the PagSeguroServiceException class
        /// </summary>
        /// <param name="message"></param>
        public PagSeguroServiceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PagSeguroServiceException class
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public PagSeguroServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PagSeguroServiceException class
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="innerException"></param>
        public PagSeguroServiceException(HttpStatusCode statusCode, Exception innerException) :
            base(String.Format(CultureInfo.InvariantCulture, "HttpStatusCode: {0} ({1})", statusCode, (int)statusCode), innerException)
        {
            this.StatusCode = statusCode;
        }

        private PagSeguroServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.StatusCode = (HttpStatusCode)info.GetValue(PagSeguroServiceException.HTTP_STATUS_CODE_FIELD, typeof(HttpStatusCode));
            this._errors = (List<PagSeguroServiceError>)info.GetValue(PagSeguroServiceException.ERRORS_FIELD, typeof(List<PagSeguroServiceError>));
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the target object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(PagSeguroServiceException.HTTP_STATUS_CODE_FIELD, this.StatusCode);
            info.AddValue(PagSeguroServiceException.ERRORS_FIELD, this.Errors);
        }

        /// <summary>
        /// List of errors returned by the PagSeguro web service
        /// </summary>
        public IList<PagSeguroServiceError> Errors
        {
            get
            {
                if (this._errors == null)
                {
                    this._errors = new List<PagSeguroServiceError>();
                }
                return this._errors.AsReadOnly();
            }
        }

        /// <summary>
        /// Http status code
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a textual representation of this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(base.ToString());
            builder.Append(CR_LF);
            foreach (PagSeguroServiceError error in this.Errors)
            {
                builder.Append(error.ToString()).Append(CR_LF);
            }
            return builder.ToString();
        }
    }
}
