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
using System.Collections.ObjectModel;

namespace Uol.PagSeguro
{
    /// <summary>
    /// Represents a page of transactions returned by the transaction search service
    /// </summary>
    public class TransactionSearchResult
    {
        private List<TransactionSummary> items = new List<TransactionSummary>();

        /// <summary>
        /// Date/time when this search was executed
        /// </summary>
        public DateTime Date
        {
            get;
            internal set;
        }

        /// <summary>
        /// Current page
        /// </summary>
        public int CurrentPage
        {
            get;
            internal set;
        }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages
        {
            get;
            internal set;
        }

        internal IList<TransactionSummary> Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// Transactions in this page
        /// </summary>
        public ReadOnlyCollection<TransactionSummary> Transactions
        {
            get
            {
                return this.items.AsReadOnly();
            }
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.GetType().Name);
            builder.Append('(');
            builder.Append("Date=").Append(this.Date).Append(", ");
            builder.Append("CurrentPage=").Append(this.CurrentPage).Append(", ");
            builder.Append("TotalPages=").Append(this.TotalPages).Append(", ");
            builder.Append("Transactions in this page=").Append(this.Transactions.Count);
            builder.Append(')');
            return builder.ToString();
        }
    }
}
