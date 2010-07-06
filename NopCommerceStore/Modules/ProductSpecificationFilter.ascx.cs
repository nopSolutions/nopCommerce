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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Content.Polls;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ProductSpecificationFilterControl : BaseNopUserControl
    {
        #region Utilities
        protected void BindData()
        {
            var alreadyFilteredOptions = getAlreadyFilteredSpecs();
            var notFilteredOptions = getNotFilteredSpecs();

            if (alreadyFilteredOptions.Count > 0 || notFilteredOptions.Count > 0)
            {
                if (alreadyFilteredOptions.Count > 0)
                {
                    rptAlreadyFilteredPSO.DataSource = alreadyFilteredOptions;
                    rptAlreadyFilteredPSO.DataBind();

                    string url = CommonHelper.GetThisPageUrl(true);
                    string[] alreadyFilteredSpecsQueryStringParams = getAlreadyFilteredSpecsQueryStringParams();
                    foreach (string qsp in alreadyFilteredSpecsQueryStringParams)
                    {
                        //little hack here because we get encoded query strong params from CommonHelper.GetThisPageUrl();
                        string qsp2 = HttpUtility.UrlPathEncode(qsp);
                        string qsp3 = HttpUtility.UrlEncodeUnicode(qsp);
                        url = CommonHelper.RemoveQueryString(url, qsp2);
                        url = CommonHelper.RemoveQueryString(url, qsp3);
                    }
                    url = excludeQueryStringParams(url);
                    hlRemoveFilter.NavigateUrl = url;
                }
                else
                {
                    pnlAlreadyFilteredPSO.Visible = false;
                    pnlRemoveFilter.Visible = false;
                }

                if (notFilteredOptions.Count > 0)
                {
                    rptFilterByPSO.DataSource = notFilteredOptions;
                    rptFilterByPSO.DataBind();
                }
                else
                {
                    pnlPSOSelector.Visible = false;
                }

            }
            else
            {
                Visible = false;
            }
        }

        protected List<SpecificationAttributeOptionFilter> getAlreadyFilteredSpecs()
        {
            var result = new List<SpecificationAttributeOptionFilter>();

            string[] queryStringParams = getAlreadyFilteredSpecsQueryStringParams();
            foreach (string qsp in queryStringParams)
            {
                int id = 0;
                int.TryParse(Request.QueryString[qsp], out id);
                var sao = SpecificationAttributeManager.GetSpecificationAttributeOptionById(id);
                if (sao != null)
                {
                    var sa = sao.SpecificationAttribute;
                    if (sa != null)
                    {
                        result.Add(new SpecificationAttributeOptionFilter
                        {
                            SpecificationAttributeId = sa.SpecificationAttributeId,
                            SpecificationAttributeName = sa.LocalizedName,
                            DisplayOrder = sa.DisplayOrder,
                            SpecificationAttributeOptionId = sao.SpecificationAttributeOptionId,
                            SpecificationAttributeOptionName = sao.LocalizedName
                        });
                    }
                }
            }

            return result;
        }

        protected List<SpecificationAttributeOptionFilter> getNotFilteredSpecs()
        {
            //get all
            var result = SpecificationAttributeManager.GetSpecificationAttributeOptionFilter(this.CategoryId);
           
            //remove already filtered
            var alreadyFilteredOptions = getAlreadyFilteredSpecs();
            foreach (var saof1 in alreadyFilteredOptions)
            {
                var query = from s
                                in result
                            where s.SpecificationAttributeId == saof1.SpecificationAttributeId
                            select s;

                List<SpecificationAttributeOptionFilter> toRemove = query.ToList();

                foreach (var saof2 in toRemove)
                {
                    result.Remove(saof2);
                }
            }
            return result;
        }

        protected string[] getAlreadyFilteredSpecsQueryStringParams()
        {
            var result = new List<string>();

            var reservedQueryStringParamsSplitted = new List<string>();
            if (!String.IsNullOrEmpty(this.ReservedQueryStringParams))
            {
                reservedQueryStringParamsSplitted = this.ReservedQueryStringParams.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            foreach (string qsp in Request.QueryString)
            {
                if (!String.IsNullOrEmpty(qsp))
                {
                    string _qsp = qsp.ToLowerInvariant();
                    if (!reservedQueryStringParamsSplitted.Contains(_qsp))
                    {
                        if (!result.Contains(_qsp))
                            result.Add(_qsp);
                    }
                }
            }
            return result.ToArray();
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

        private string lastSA = string.Empty;
        protected string addSpecificationAttribute()
        {
            //Get the data field value of interest for this row   
            string currentSA = Eval("SpecificationAttributeName").ToString();

            //See if there's been a change in value
            if (lastSA != currentSA)
            {
                lastSA = currentSA;
                return String.Format("<tr class=\"group\"><td>{0}</td></tr>", Server.HtmlEncode(currentSA));
            }
            else
            {
                return String.Empty;
            }
        }

        #endregion

        #region Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected void rptFilterByPSO_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var row = e.Item.DataItem as SpecificationAttributeOptionFilter;
                var lnkFilter = e.Item.FindControl("lnkFilter") as HyperLink;
                if (lnkFilter != null)
                {
                    string name = row.SpecificationAttributeName.Replace(" ", "");
                    string url = CommonHelper.ModifyQueryString(CommonHelper.GetThisPageUrl(true), name + "=" + row.SpecificationAttributeOptionId, null);
                    url = excludeQueryStringParams(url);
                    lnkFilter.NavigateUrl = url;
                }
            }
        }

        protected void rptAlreadyFilteredPSO_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var row = e.Item.DataItem as SpecificationAttributeOptionFilter;
            }
        }

        #endregion

        #region Methods
        public List<int> GetAlreadyFilteredSpecOptionIds()
        {
            var result = new List<int>();
            var filterOptions = getAlreadyFilteredSpecs();
            foreach (var saof in filterOptions)
            {
                if (!result.Contains(saof.SpecificationAttributeOptionId))
                    result.Add(saof.SpecificationAttributeOptionId);
            }
            return result;
        }
        #endregion

        #region Properties
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

        public string ReservedQueryStringParams
        {
            get
            {
                if (ViewState["ReservedQueryStringParams"] == null)
                    return string.Empty;
                else
                    return ((string)ViewState["ReservedQueryStringParams"]).ToLowerInvariant();
            }
            set
            {
                ViewState["ReservedQueryStringParams"] = value;
            }
        }

        /// <summary>
        /// Category identifier
        /// </summary>
        public int CategoryId
        {
            get
            {
                if (ViewState["CategoryId"] == null)
                    return 0;
                else
                    return (int)ViewState["CategoryId"];
            }
            set
            {
                ViewState["CategoryId"] = value;
            }
        }
        #endregion
    }
}