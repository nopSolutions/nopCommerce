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
using System.Security.Permissions;

namespace Uol.PagSeguro
{
    /// <summary>
    /// Represents a PagSeguro web service error
    /// </summary>
    [Serializable]
    public sealed class PagSeguroServiceError : ISerializable
    {
        private const string CodeField = "Code";
        private const string MessageField = "Message";

        /// <summary>
        /// Initializes a new instance of the PagSeguroServiceError class
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public PagSeguroServiceError(string code, string message)
        {
            if (code == null)
                throw new ArgumentNullException("code");
            if (message == null)
                throw new ArgumentNullException("message");

            this.Code = code;
            this.Message = message;
        }

        private PagSeguroServiceError(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            this.Code = info.GetString(PagSeguroServiceError.CodeField);
            this.Message = info.GetString(PagSeguroServiceError.MessageField);
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the target object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            info.AddValue(PagSeguroServiceError.CodeField, this.Code);
            info.AddValue(PagSeguroServiceError.MessageField, this.Message);
        }

        /// <summary>
        /// Error code
        /// </summary>
        public string Code
        {
            get;
            private set;
        }

        /// <summary>
        /// Error description
        /// </summary>
        public string Message
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
            StringBuilder builder = new StringBuilder(this.Code);
            builder.Append("=");
            builder.Append(this.Message);
            return builder.ToString();
        }
    }
}
