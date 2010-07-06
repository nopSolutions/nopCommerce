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
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Configuration;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic
{
    /// <summary>
    /// Represents a EventContext
    /// </summary>
    public partial class EventContext
    {
        #region Nested

        public delegate void CustomerEventHandler(object sender, CustomerEventArgs e);

        public delegate void OrderEventHandler(object sender, OrderEventArgs e);

        public delegate void ProductEventHandler(object sender, ProductEventArgs e);

        public delegate void ProductVariantEventHandler(object sender, ProductVariantEventArgs e);

        #endregion

        #region Ctor

        /// <summary>
        /// Creates a new instance of the EventContext class
        /// </summary>
        private EventContext()
        {

        }

        #endregion

        #region Methods

        public virtual void OnCustomerCreated(object source, CustomerEventArgs e)
        {
            if (this.CustomerCreated != null)
                this.CustomerCreated(source, e);
        }

        public virtual void OnCustomerUpdated(object source, CustomerEventArgs e)
        {
            if (this.CustomerUpdated != null)
                this.CustomerUpdated(source, e);
        }

        public virtual void OnOrderCreated(object source, OrderEventArgs e)
        {
            if (this.OrderCreated != null)
                this.OrderCreated(source, e);
        }

        public virtual void OnOrderPlaced(object source, OrderEventArgs e)
        {
            if (this.OrderPlaced != null)
                this.OrderPlaced(source, e);
        }

        public virtual void OnOrderUpdated(object source, OrderEventArgs e)
        {
            if (this.OrderUpdated != null)
                this.OrderUpdated(source, e);
        }

        public virtual void OnProductCreated(object source, ProductEventArgs e)
        {
            if (this.ProductCreated != null)
                this.ProductCreated(source, e);
        }

        public virtual void OnProductUpdated(object source, ProductEventArgs e)
        {
            if (this.ProductUpdated != null)
                this.ProductUpdated(source, e);
        }

        public virtual void OnProductVariantCreated(object source, ProductVariantEventArgs e)
        {
            if (this.ProductVariantCreated != null)
                this.ProductVariantCreated(source, e);
        }

        public virtual void OnProductVariantUpdated(object source, ProductVariantEventArgs e)
        {
            if (this.ProductVariantUpdated != null)
                this.ProductVariantUpdated(source, e);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the EventContext, which can be used to retrieve information about current context.
        /// </summary>
        public static EventContext Current
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    object data = Thread.GetData(Thread.GetNamedDataSlot("NopEventContext"));
                    if (data != null)
                    {
                        return (EventContext)data;
                    }
                    EventContext context = new EventContext();
                    Thread.SetData(Thread.GetNamedDataSlot("NopEventContext"), context);
                    return context;
                }
                if (HttpContext.Current.Items["NopEventContext"] == null)
                {
                    EventContext context = new EventContext();
                    HttpContext.Current.Items.Add("NopEventContext", context);
                    return context;
                }
                return (EventContext)HttpContext.Current.Items["NopEventContext"];
            }
        }

        public event CustomerEventHandler CustomerCreated;
        public event CustomerEventHandler CustomerUpdated;

        public event OrderEventHandler OrderCreated;
        public event OrderEventHandler OrderPlaced;
        public event OrderEventHandler OrderUpdated;

        public event ProductEventHandler ProductCreated;
        public event ProductEventHandler ProductUpdated;

        public event ProductVariantEventHandler ProductVariantCreated;
        public event ProductVariantEventHandler ProductVariantUpdated;

        #endregion
    }
}
