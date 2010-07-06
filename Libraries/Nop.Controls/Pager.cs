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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Controls
{
    /// <summary>
    /// Pager control
    /// </summary>
    [ParseChildren(true)]
    public partial class Pager : Control, IAttributeAccessor, INamingContainer
    {
        #region Fields
        private int _pageIndex = -2;
        private bool _isDataBound;
        private System.Web.UI.AttributeCollection atts;
        private StateBag attState;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the Pager class.
        /// </summary>
        public Pager()
        {
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Binds default pager content
        /// </summary>
        /// <param name="control">Control</param>
        protected virtual void BindDefaultContent(Control control)
        {
            if ((!this.ShowTotalSummary || (this.TotalPages <= 0)) && (!this.ShowPagerItems || (this.TotalPages <= 1)))
            {
                this.AutomatedVisible = false;
            }
            else
            {
                if (this.ShowTotalSummary && (this.TotalPages > 0))
                {
                    this.BindTotalSummary(control);
                }
                if (this.ShowPagerItems && (this.TotalPages > 1))
                {
                    if (this.ShowFirst)
                    {
                        this.BindFirst(control);
                    }
                    if (this.ShowPrevious)
                    {
                        this.BindPrevious(control);
                    }
                    if (this.ShowIndividualPages)
                    {
                        this.BindPages(control);
                    }
                    if (this.ShowNext)
                    {
                        this.BindNext(control);
                    }
                    if (this.ShowLast)
                    {
                        this.BindLast(control);
                    }
                }
            }
        }

        /// <summary>
        /// Bind "first"
        /// </summary>
        /// <param name="control">Control</param>
        protected virtual void BindFirst(Control control)
        {
            if ((this.PageIndex >= 3) && (this.TotalPages > this.IndividualPagesDisplayedCount))
            {
                HyperLink child = new HyperLink();
                child.Text = this.FirstButtonText;
                child.NavigateUrl = this.GetPageNavigateUrl(1);
                control.Controls.Add(child);

                if ((this.ShowIndividualPages || (this.ShowPrevious && (this.PageIndex > 0))) || this.ShowLast)
                {
                    control.Controls.Add(new LiteralControl("&nbsp;...&nbsp;"));
                }
            }
        }

        /// <summary>
        /// Bind "last"
        /// </summary>
        /// <param name="control">Control</param>
        protected virtual void BindLast(Control control)
        {
            if (((this.PageIndex + 3) < this.TotalPages) && (this.TotalPages > this.IndividualPagesDisplayedCount))
            {
                if (this.ShowIndividualPages || (this.ShowNext && ((this.PageIndex + 1) < this.TotalPages)))
                {
                    control.Controls.Add(new LiteralControl("&nbsp;...&nbsp;"));
                }

                HyperLink child = new HyperLink();
                child.Text = this.LastButtonText;
                child.NavigateUrl = this.GetPageNavigateUrl(this.TotalPages);
                control.Controls.Add(child);
            }
        }

        /// <summary>
        /// Bind "next"
        /// </summary>
        /// <param name="control">Control</param>
        protected virtual void BindNext(Control control)
        {
            if ((this.PageIndex + 1) < this.TotalPages)
            {
                if (this.ShowIndividualPages)
                {
                    control.Controls.Add(new LiteralControl("&nbsp;"));
                }

                HyperLink child = new HyperLink();
                child.Text = this.NextButtonText;
                child.NavigateUrl = this.GetPageNavigateUrl(this.PageIndex + 2);
                control.Controls.Add(child);
            }
        }

        /// <summary>
        /// Bind pages
        /// </summary>
        /// <param name="control">Control</param>
        protected virtual void BindPages(Control control)
        {
            int firstIndividualPageIndex = this.GetFirstIndividualPageIndex();
            int lastIndividualPageIndex = this.GetLastIndividualPageIndex();
            for (int i = firstIndividualPageIndex; i <= lastIndividualPageIndex; i++)
            {
                if (this.PageIndex == i)
                {
                    int num5 = i + 1;
                    control.Controls.Add(new LiteralControl(String.Format("<span>{0}</span>", num5.ToString())));
                }
                else
                {
                    HyperLink child = new HyperLink();
                    child.EnableViewState = false;
                    child.Text = (i + 1).ToString();
                    child.NavigateUrl = this.GetPageNavigateUrl(i + 1);
                    control.Controls.Add(child);
                }
                if (i < lastIndividualPageIndex)
                {
                    control.Controls.Add(new LiteralControl(" "));
                }
            }
        }

        /// <summary>
        /// Bind "previous"
        /// </summary>
        /// <param name="control">Control</param>
        protected virtual void BindPrevious(Control control)
        {
            if (this.PageIndex > 0)
            {
                HyperLink child = new HyperLink();
                child.Text = this.PreviousButtonText;
                child.NavigateUrl = this.GetPageNavigateUrl(this.PageIndex);
                control.Controls.Add(child);

                if ((this.ShowIndividualPages || this.ShowLast) || (this.ShowNext && ((this.PageIndex + 1) < this.TotalPages)))
                {
                    control.Controls.Add(new LiteralControl("&nbsp;"));
                }
            }
        }

        /// <summary>
        /// Bind "total summary"
        /// </summary>
        /// <param name="control">Control</param>
        protected virtual void BindTotalSummary(Control control)
        {
            control.Controls.Add(new LiteralControl(string.Format(this.CurrentPageText, this.PageIndex + 1, this.TotalPages, this.TotalRecords)));
            control.Controls.Add(new LiteralControl(" "));
        }

        /// <summary>
        /// Creates the control hierarchy used to render the Pager control.
        /// </summary>
        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            base.ClearChildViewState();
        }

        /// <summary>
        /// Calls the DataBind method if the data-bound control is marked to require binding.
        /// </summary>
        protected virtual void EnsureDataBound()
        {
            if (!this._isDataBound)
            {
                this.DataBind();
            }
        }

        /// <summary>
        /// Gets first individual page index
        /// </summary>
        /// <returns>Page index</returns>
        protected virtual int GetFirstIndividualPageIndex()
        {
            if ((this.TotalPages < this.IndividualPagesDisplayedCount) || ((this.PageIndex - (this.IndividualPagesDisplayedCount / 2)) < 0))
            {
                return 0;
            }
            if ((this.PageIndex + (this.IndividualPagesDisplayedCount / 2)) >= this.TotalPages)
            {
                return (this.TotalPages - this.IndividualPagesDisplayedCount);
            }
            return (this.PageIndex - (this.IndividualPagesDisplayedCount / 2));
        }

        /// <summary>
        /// Get initial page index
        /// </summary>
        /// <returns>Page index</returns>
        protected virtual int GetInitialPageIndex()
        {
            return (CommonHelper.QueryStringInt(this.QueryStringProperty, 0) - 1);
        }

        /// <summary>
        /// Get last individual page index
        /// </summary>
        /// <returns>Page index</returns>
        protected virtual int GetLastIndividualPageIndex()
        {
            int num = this.IndividualPagesDisplayedCount / 2;
            if ((this.IndividualPagesDisplayedCount % 2) == 0)
            {
                num--;
            }
            if ((this.TotalPages < this.IndividualPagesDisplayedCount) || ((this.PageIndex + num) >= this.TotalPages))
            {
                return (this.TotalPages - 1);
            }
            if ((this.PageIndex - (this.IndividualPagesDisplayedCount / 2)) < 0)
            {
                return (this.IndividualPagesDisplayedCount - 1);
            }
            return (this.PageIndex + num);
        }

        /// <summary>
        /// Get bavigate Url of the specified URL
        /// </summary>
        /// <param name="pageIndex">Page index of the page</param>
        /// <returns>Url</returns>
        protected virtual string GetPageNavigateUrl(int pageIndex)
        {
            return CommonHelper.ModifyQueryString(CommonHelper.GetThisPageUrl(true), this.QueryStringProperty + "=" + pageIndex.ToString(), null);
        }

        /// <summary>
        /// Raises the Load event and performs other initialization. 
        /// </summary>
        /// <param name="e">The EventArgs object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            int initialPageIndex = this.GetInitialPageIndex();
            if (initialPageIndex >= 0)
            {
                this.PageIndex = initialPageIndex;
                this.PageIndexChanged(this, new PagerEventArgs() { PageSize = this.PageSize, PageIndex = initialPageIndex, TotalRecords = this.TotalRecords });
            }
            base.OnLoad(e);
        }

        /// <summary>
        /// Raises event indicating that page index is changed
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">Arguments</param>
        protected void PageIndexChanged(Pager sender, PagerEventArgs e)
        {
            if (this.OnPageIndexChanged != null)
            {
                this.OnPageIndexChanged(sender, e);
            }
        }

        /// <summary>
        /// Raises the DataBinding event.
        /// </summary>
        /// <param name="e">An EventArgs that contains event data.</param>
        protected override void OnDataBinding(EventArgs e)
        {
            this.EnsureChildControls();
            base.OnDataBinding(e);
        }

        /// <summary>
        /// Raises the OnPreRender event
        /// </summary>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!this._isDataBound)
            {
                this.DataBind();
            }
        }

        /// <summary>
        /// Writes the Pager control content to the specified HtmlTextWriter object for display on the client.
        /// </summary>
        /// <param name="writer">An HtmlTextWriter that represents the output stream to render HTML content on the client.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            string tag = "div";
            writer.Write("<{0}", tag);
            if (!string.IsNullOrEmpty(this.CssClass))
            {
                writer.Write(" class=\"{0}\"", this.CssClass);
            }
            if ((this.Attributes != null) && (this.Attributes.Count > 0))
            {
                foreach (string str2 in this.Attributes.Keys)
                {
                    writer.Write(" {0}=\"{1}\"", str2, this.Attributes[str2]);
                }
            }
            writer.Write(">");

            base.Render(writer);

            writer.Write("</{0}>", tag);
        }

        /// <summary>
        /// Updates visible property
        /// </summary>
        protected void UpdateVisible()
        {
            bool vis = true;
            if (this.ViewState["Visible"] != null)
                vis = (bool)this.ViewState["Visible"];
            base.Visible = vis && this.AutomatedVisible;
        }

        /// <summary>
        /// Retrieves the specified attribute property from the control
        /// </summary>
        /// <param name="key">A String that represents the name of the server control attribute</param>
        /// <returns>The value of the specified attribute</returns>
        string IAttributeAccessor.GetAttribute(string key)
        {
            return this.Attributes[key];
        }

        /// <summary>
        /// Designates an attribute and its value to assign to the control
        /// </summary>
        /// <param name="key">The name of the attribute to be set.</param>
        /// <param name="value">The value assigned to the attribute.</param>
        void IAttributeAccessor.SetAttribute(string key, string value)
        {
            this.Attributes.Add(key, value);
        }

        #endregion

        #region Methods
        /// <summary>
        /// Binds the control and all its child controls to the specified data source.
        /// </summary>
        public override void DataBind()
        {
            if ((this.TotalRecords > 0) && ((this.TotalPages - 1) < this.PageIndex))
            {
                this.PageIndex = this.TotalPages - 1;
                this.OnPageIndexChanged(this, new PagerEventArgs() { PageSize = this.PageSize, PageIndex = this.PageIndex, TotalRecords= this.TotalRecords });
            }

            this.BindDefaultContent(this);
            base.DataBind();
            this._isDataBound = true;
        }
        #endregion

        #region Events

        /// <summary>
        /// This event occurs when page index is changed
        /// </summary>
        public event PagerEventHandler OnPageIndexChanged;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a query string
        /// </summary>
        public string QueryStringProperty
        {
            get
            {
                return (((string)this.ViewState["QueryStringProperty"]) ?? "PageIndex").ToLowerInvariant();
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                this.ViewState["QueryStringProperty"] = value.ToLowerInvariant();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use validation
        /// </summary>
        public virtual bool CausesValidation
        {
            get
            {
                if (this.ViewState["CausesValidation"] == null)
                    return true;
                else
                    return (bool)this.ViewState["CausesValidation"];
            }
            set
            {
                this.ViewState["CausesValidation"] = value;
            }
        }

        /// <summary>
        /// Gets the current paeg index
        /// </summary>
        public virtual int CurrentPage
        {
            get
            {
                return (this.PageIndex + 1);
            }
        }

        /// <summary>
        /// Gets or sets a count of individual pages to be displayed
        /// </summary>
        public virtual int IndividualPagesDisplayedCount
        {
            get
            {
                if (this.ViewState["IndividualPagesDisplayedCount"] == null)
                    return 5;
                else
                    return (int)this.ViewState["IndividualPagesDisplayedCount"];
            }
            set
            {
                this.ViewState["IndividualPagesDisplayedCount"] = value;
            }
        }

        /// <summary>
        /// Gets the current page index
        /// </summary>
        public virtual int PageIndex
        {
            get
            {
                if (this._pageIndex == -2)
                {
                    int initialPageIndex = this.GetInitialPageIndex();
                    if (initialPageIndex >= 0)
                    {
                        this.PageIndex = initialPageIndex;
                        this.PageIndexChanged(this, new PagerEventArgs() { PageSize = this.PageSize, PageIndex = initialPageIndex, TotalRecords = this.TotalRecords });
                    }
                }
                if (this._pageIndex < 0)
                {
                    return 0;
                }
                return this._pageIndex;
            }
            set
            {
                this._pageIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets a page size
        /// </summary>
        public virtual int PageSize
        {
            get
            {
                if (this.ViewState["PageSize"] == null)
                    return 10;
                else
                    return (int)this.ViewState["PageSize"];
            }
            set
            {
                this.ViewState["PageSize"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show "first"
        /// </summary>
        public virtual bool ShowFirst
        {
            get
            {
                if (this.ViewState["ShowFirst"] == null)
                    return true;
                else
                    return (bool)this.ViewState["ShowFirst"];
            }
            set
            {
                this.ViewState["ShowFirst"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show "individual pages"
        /// </summary>
        public virtual bool ShowIndividualPages
        {
            get
            {
                if (this.ViewState["ShowIndividualPages"] == null)
                    return true;
                else
                    return (bool)this.ViewState["ShowIndividualPages"];
            }
            set
            {
                this.ViewState["ShowIndividualPages"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show "last"
        /// </summary>
        public virtual bool ShowLast
        {
            get
            {
                if (this.ViewState["ShowLast"] == null)
                    return true;
                else
                    return (bool)this.ViewState["ShowLast"];
            }
            set
            {
                this.ViewState["ShowLast"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show "next"
        /// </summary>
        public virtual bool ShowNext
        {
            get
            {
                if (this.ViewState["ShowNext"] == null)
                    return true;
                else
                    return (bool)this.ViewState["ShowNext"];
            }
            set
            {
                this.ViewState["ShowNext"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show pager items
        /// </summary>
        public virtual bool ShowPagerItems
        {
            get
            {
                if (this.ViewState["ShowPagerItems"] == null)
                    return true;
                else
                    return (bool)this.ViewState["ShowPagerItems"];
            }
            set
            {
                this.ViewState["ShowPagerItems"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show "previous"
        /// </summary>
        public virtual bool ShowPrevious
        {
            get
            {
                if (this.ViewState["ShowPrevious"] == null)
                    return true;
                else
                    return (bool)this.ViewState["ShowPrevious"];
            }
            set
            {
                this.ViewState["ShowPrevious"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show "total summary"
        /// </summary>
        public virtual bool ShowTotalSummary
        {
            get
            {
                if (this.ViewState["ShowTotalSummary"] == null)
                    return false;
                else
                    return (bool)this.ViewState["ShowTotalSummary"];
            }
            set
            {
                this.ViewState["ShowTotalSummary"] = value;
            }
        }

        /// <summary>
        /// Gets a total pages count
        /// </summary>
        public virtual int TotalPages
        {
            get
            {
                if ((this.TotalRecords == 0) || (this.PageSize == 0))
                {
                    return 0;
                }
                int num = this.TotalRecords / this.PageSize;
                if ((this.TotalRecords % this.PageSize) > 0)
                {
                    num++;
                }
                return num;
            }
        }

        /// <summary>
        /// Gets or sets a total records count
        /// </summary>
        public virtual int TotalRecords
        {
            get
            {
                if (this.ViewState["TotalRecords"] == null)
                    return 0;
                else
                    return (int)this.ViewState["TotalRecords"];
            }
            set
            {
                this.ViewState["TotalRecords"] = value;
            }
        }

        /// <summary>
        /// Contains the list of attributes for the pager.
        /// </summary>
        public virtual System.Web.UI.AttributeCollection Attributes
        {
            get
            {
                if (this.atts == null)
                {
                    this.attState = new StateBag(true);
                    this.atts = new System.Web.UI.AttributeCollection(this.attState);
                }
                return this.atts;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to automated visible
        /// </summary>
        protected virtual bool AutomatedVisible
        {
            get
            {
                if (this.ViewState["AutomatedVisible"] == null)
                    return true;
                else
                    return (bool)this.ViewState["AutomatedVisible"];
            }
            set
            {
                this.ViewState["AutomatedVisible"] = value;
                this.UpdateVisible();
            }
        }

        /// <summary>
        /// Gets or sets the cascading style sheet (CSS) file that specifies the display styles
        /// </summary>
        public virtual string CssClass
        {
            get
            {
                return (((string)this.ViewState["CssClass"]) ?? string.Empty);
            }
            set
            {
                this.ViewState["CssClass"] = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the pager is visible
        /// </summary>
        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                this.ViewState["Visible"] = value;
                this.UpdateVisible();
            }
        }

        /// <summary>
        /// Gets or sets the first button text
        /// </summary>
        public string FirstButtonText
        {
            get
            {
                return (((string)this.ViewState["FirstButtonText"]) ?? "First");
            }
            set
            {
                this.ViewState["FirstButtonText"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the last button text
        /// </summary>
        public string LastButtonText
        {
            get
            {
                return (((string)this.ViewState["LastButtonText"]) ?? "Last");
            }
            set
            {
                this.ViewState["LastButtonText"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the next button text
        /// </summary>
        public string NextButtonText
        {
            get
            {
                return (((string)this.ViewState["NextButtonText"]) ?? "Next");
            }
            set
            {
                this.ViewState["NextButtonText"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the previous button text
        /// </summary>
        public string PreviousButtonText
        {
            get
            {
                return (((string)this.ViewState["PreviousButtonText"]) ?? "Previous");
            }
            set
            {
                this.ViewState["PreviousButtonText"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the current page text
        /// </summary>
        public string CurrentPageText
        {
            get
            {
                return (((string)this.ViewState["CurrentPageText"]) ?? "Page");
            }
            set
            {
                this.ViewState["CurrentPageText"] = value;
            }
        }
        #endregion
    }
}