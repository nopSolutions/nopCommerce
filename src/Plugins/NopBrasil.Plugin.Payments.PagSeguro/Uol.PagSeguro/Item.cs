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
    /// Represents a product/item in a transaction
    /// </summary>
    public class Item
    {
        internal Item()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Item class
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="quantity"></param>
        /// <param name="amount"></param>
        public Item(string id, string description, int quantity, decimal amount) :
            this(id, description, quantity, amount, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Item class
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="quantity"></param>
        /// <param name="amount"></param>
        /// <param name="weight"></param>
        public Item(string id, string description, int quantity, decimal amount, long weight) :
            this(id, description, quantity, amount, weight, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Item class
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="quantity"></param>
        /// <param name="amount"></param>
        /// <param name="shippingCost"></param>
        public Item(string id, string description, int quantity, decimal amount, decimal shippingCost) :
            this(id, description, quantity, amount, null, shippingCost)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Item class
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="quantity"></param>
        /// <param name="amount"></param>
        /// <param name="weight"></param>
        /// <param name="shippingCost"></param>
        public Item(string id, string description, int quantity, decimal amount, long weight, decimal shippingCost)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");
            if (String.IsNullOrEmpty(description))
                throw new ArgumentNullException("description");

            this.Id = id;
            this.Description = description;
            this.Quantity = quantity;
            this.Amount = amount;
            this.Weight = weight;
            this.ShippingCost = shippingCost;
        }

        private Item(string id, string description, int quantity, decimal amount, long? weight, decimal? shippingCost)
        {
            if (String.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");
            if (String.IsNullOrEmpty(description))
                throw new ArgumentNullException("description");

            this.Id = id;
            this.Description = description;
            this.Quantity = quantity;
            this.Amount = amount;
            this.Weight = weight;
            this.ShippingCost = shippingCost;
        }

        /// <summary>
        /// Product identifier, such as SKU
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Product description
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Quantity
        /// </summary>
        public int Quantity
        {
            get;
            set;
        }

        /// <summary>
        /// Product unit price 
        /// </summary>
        public decimal Amount
        {
            get;
            set;
        }

        /// <summary>
        /// Single unit weight, in grams
        /// </summary>
        public long? Weight
        {
            get;
            set;
        }

        /// <summary>
        /// Single unit shipping cost
        /// </summary>
        public decimal? ShippingCost
        {
            get;
            set;
        }
    }
}
