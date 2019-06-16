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

namespace Uol.PagSeguro
{
    /// <summary>
    /// Represents a name value pair that composes a credential
    /// </summary>
    public class CredentialsNameValuePair
    {
        /// <summary>
        /// Initializes a new instance of the CredentialsNameValuePair class
        /// </summary>
        public CredentialsNameValuePair(string name, string value)
        {
            if (String.IsNullOrEmpty("name"))
                throw new ArgumentNullException("name");
            if (String.IsNullOrEmpty("value"))
                throw new ArgumentNullException("value");

            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }

        /// <summary>
        /// Value
        /// </summary>
        public string Value
        {
            get;
            internal set;
        }
    }
}
