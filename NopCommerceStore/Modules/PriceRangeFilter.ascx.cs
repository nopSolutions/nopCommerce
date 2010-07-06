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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Content.Polls;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common.Utils;


namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class PriceRangeFilterControl : BaseNopUserControl
    {
        private const string PRICE_CSSCLASSNAME = "PriceRange";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected void BindData()
        {
            var priceRangeList = GetPriceRangeList();
            if (priceRangeList.Count > 0)
            {
                if (this.SelectedPriceRange == null)
                {
                    pnlPriceRangeSelector.Visible = true;
                    pnlSelectedPriceRange.Visible = false;
                    rptrPriceRange.DataSource = priceRangeList;
                    rptrPriceRange.DataBind();
                }
                else
                {
                    pnlPriceRangeSelector.Visible = false;
                    pnlSelectedPriceRange.Visible = true;

                    string url = CommonHelper.RemoveQueryString(CommonHelper.GetThisPageUrl(true), this.QueryStringProperty);
                    url = excludeQueryStringParams(url);
                    hlRemoveFilter.NavigateUrl = url;

                    lblSelectedPriceRange.Text = getPriceRangeString(this.SelectedPriceRange.From, this.SelectedPriceRange.To);
                }
            }
            else
            {
                this.Visible = false;
            }
        }

        protected string getPriceRangeString(decimal? From, decimal? To)
        {
            string result = string.Empty;
            if (!From.HasValue)
            {
                string toString = PriceHelper.FormatPrice(To.Value, true, false);
                result = string.Format(GetLocaleResourceString("Common.PriceRangeFilter.Under"), string.Format("<span class=\"{0}\">{1}</span>", PRICE_CSSCLASSNAME, toString));
            }
            else if (!To.HasValue)
            {
                string fromString = PriceHelper.FormatPrice(From.Value, true, false);
                result = string.Format(GetLocaleResourceString("Common.PriceRangeFilter.Over"), string.Format("<span class=\"{0}\">{1}</span>", PRICE_CSSCLASSNAME, fromString));
            }
            else
            {
                string fromString = PriceHelper.FormatPrice(From.Value, true, false);
                string toString = PriceHelper.FormatPrice(To.Value, true, false);
                result = string.Format("<span class=\"{0}\">{1}</span> - <span class=\"{0}\">{2}</span>", PRICE_CSSCLASSNAME, fromString, toString);
            }
            return result;
        }

        protected string excludeQueryStringParams(string url)
        {
            if (!String.IsNullOrEmpty(this.ExcludedQueryStringParams))
            {
                string[] excludedQueryStringParamsSplitted = this.ExcludedQueryStringParams.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string exclude in excludedQueryStringParamsSplitted)
                {
                    url = CommonHelper.RemoveQueryString(url, exclude);
                }
            }

            return url;
        }

        protected void rptrPriceRange_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var priceRange = e.Item.DataItem as PriceRange;
                var hlPriceRange = e.Item.FindControl("hlPriceRange") as HyperLink;

                hlPriceRange.Text = getPriceRangeString(priceRange.From, priceRange.To);

                string fromQuery = string.Empty;
                if (priceRange.From.HasValue)
                {
                    fromQuery = priceRange.From.Value.ToString(new CultureInfo("en-US"));
                }
                string toQuery = string.Empty;
                if (priceRange.To.HasValue)
                {
                    toQuery = priceRange.To.Value.ToString(new CultureInfo("en-US"));
                }

                string url = CommonHelper.ModifyQueryString(CommonHelper.GetThisPageUrl(true), this.QueryStringProperty + "=" + fromQuery + "-" + toQuery, null);
                url = excludeQueryStringParams(url);
                hlPriceRange.NavigateUrl = url;
            }
        }

        /// <summary>
        /// Gets parsed price ranges
        /// </summary>
        protected List<PriceRange> GetPriceRangeList()
        {
            var _priceRanges = new List<PriceRange>();
            if (string.IsNullOrEmpty(this.PriceRanges))
                return _priceRanges;
            string[] rangeArray = this.PriceRanges.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str1 in rangeArray)
            {
                string[] _fromTo = str1.Trim().Split(new char[] { '-' });

                decimal? _from = null;
                if (!String.IsNullOrEmpty(_fromTo[0]) && !String.IsNullOrEmpty(_fromTo[0].Trim()))
                {
                    _from = decimal.Parse(_fromTo[0].Trim(), new CultureInfo("en-US"));
                }
                decimal? _to = null;
                if (!String.IsNullOrEmpty(_fromTo[1]) && !String.IsNullOrEmpty(_fromTo[1].Trim()))
                {
                    _to = decimal.Parse(_fromTo[1].Trim(), new CultureInfo("en-US"));
                }
                _priceRanges.Add(new PriceRange() { From = _from, To = _to });
            }
            return _priceRanges;
        }

        public string QueryStringProperty
        {
            get
            {
                return (((string)this.ViewState["QueryStringProperty"]) ?? "Price").ToLowerInvariant();
            }
            set
            {
                this.ViewState["QueryStringProperty"] = value;
            }
        }

        public string ExcludedQueryStringParams
        {
            get
            {
                if (ViewState["ExcludedQueryStringParams"] == null)
                    return string.Empty;
                else
                    return ((string)ViewState["ExcludedQueryStringParams"]).ToLowerInvariant();
            }
            set
            {
                ViewState["ExcludedQueryStringParams"] = value;
            }
        }

        public string PriceRanges
        {
            get
            {
                if (ViewState["PriceRanges"] == null)
                    return string.Empty;
                else
                    return (string)ViewState["PriceRanges"];
            }
            set { ViewState["PriceRanges"] = value; }
        }

        public PriceRange SelectedPriceRange
        {
            get
            {
                string _range = CommonHelper.QueryString(this.QueryStringProperty);
                if (String.IsNullOrEmpty(_range))
                    return null;
                string[] _fromTo = _range.Trim().Split(new char[] { '-' });
                if (_fromTo.Length == 2)
                {
                    decimal? _from = null;
                    if (!String.IsNullOrEmpty(_fromTo[0]) && !String.IsNullOrEmpty(_fromTo[0].Trim()))
                    {
                        _from = decimal.Parse(_fromTo[0].Trim(), new CultureInfo("en-US"));
                    }
                    decimal? _to = null;
                    if (!String.IsNullOrEmpty(_fromTo[1]) && !String.IsNullOrEmpty(_fromTo[1].Trim()))
                    {
                        _to = decimal.Parse(_fromTo[1].Trim(), new CultureInfo("en-US"));
                    }

                    var priceRangeList = GetPriceRangeList();
                    foreach (var pr in priceRangeList)
                    {
                        if (pr.From == _from && pr.To == _to)
                            return pr;
                    }
                }
                return null;
            }
        }
    }
}