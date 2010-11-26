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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NopSolutions.NopCommerce.Controls
{
    /// <summary>
    /// Date picker control
    /// </summary>
    public partial class NopDatePicker : Control, INamingContainer
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the DatePicker class.
        /// </summary>
        public NopDatePicker()
        {
        }
        #endregion

        #region Utilities

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            DropDownList lstDays = new DropDownList();
            lstDays.ID = "lstDays";
            lstDays.AutoPostBack = false;
            lstDays.Items.Add(new ListItem(this.DayText, "0"));
            for (int i = 1; i <= 31; i++)
            {
                lstDays.Items.Add(new ListItem(i.ToString("00"), i.ToString()));
            }
            this.Controls.Add(lstDays);

            DropDownList lstMonths = new DropDownList();
            lstMonths.ID = "lstMonths";
            lstMonths.AutoPostBack = false;
            lstMonths.Items.Add(new ListItem(this.MonthText, "0"));
            for (int i = 1; i <= 12; i++)
            {
                lstMonths.Items.Add(new ListItem(i.ToString("00"), i.ToString()));
            }
            this.Controls.Add(lstMonths);

            DropDownList lstYears = new DropDownList();
            lstYears.ID = "lstYears";
            lstYears.AutoPostBack = false;
            lstYears.Items.Add(new ListItem(this.YearText, "0"));
            for (int i = this.LastYear; i >= this.FirstYear; i--)
            {
                lstYears.Items.Add(i.ToString());
            }
            this.Controls.Add(lstYears);
        }

        #endregion

        #region Properties


        public DateTime? SelectedDate
        {
            get
            {
                DropDownList lstDays = (DropDownList)FindControl("lstDays");
                DropDownList lstMonths = (DropDownList)FindControl("lstMonths");
                DropDownList lstYears = (DropDownList)FindControl("lstYears");
                if (lstDays.SelectedIndex <= 0 || lstMonths.SelectedIndex <= 0 || lstYears.SelectedIndex <= 0)
                {
                    return null;
                }
                try
                {
                    return new DateTime(Int32.Parse(lstYears.SelectedValue), Int32.Parse(lstMonths.SelectedValue), Int32.Parse(lstDays.SelectedValue));
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                DropDownList lstDays = (DropDownList)FindControl("lstDays");
                DropDownList lstMonths = (DropDownList)FindControl("lstMonths");
                DropDownList lstYears = (DropDownList)FindControl("lstYears");

                if (value.HasValue)
                {
                    lstYears.SelectedValue = value.Value.Year.ToString();
                    lstMonths.SelectedValue = value.Value.Month.ToString();
                    lstDays.SelectedValue = value.Value.Day.ToString();
                }
                else
                {
                    lstYears.SelectedIndex = 0;
                    lstMonths.SelectedIndex = 0;
                    lstDays.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets the first year
        /// </summary>
        public int FirstYear
        {
            get
            {
                if (this.ViewState["FirstYear"] == null || (int)this.ViewState["FirstYear"] == 0)
                    return this.LastYear - 110;
                return (int)this.ViewState["FirstYear"];
            }
            set
            {
                this.ViewState["FirstYear"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the last year
        /// </summary>
        public int LastYear
        {
            get
            {
                if (this.ViewState["LastYear"] == null || (int)this.ViewState["LastYear"] == 0)
                    return DateTime.Now.Year;
                return (int)this.ViewState["LastYear"];
            }
            set
            {
                this.ViewState["LastYear"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Day text
        /// </summary>
        public string DayText
        {
            get
            {
                return (((string)this.ViewState["DayText"]) ?? "Day");
            }
            set
            {
                this.ViewState["DayText"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Month text
        /// </summary>
        public string MonthText
        {
            get
            {
                return (((string)this.ViewState["MonthText"]) ?? "Month");
            }
            set
            {
                this.ViewState["MonthText"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Year text
        /// </summary>
        public string YearText
        {
            get
            {
                return (((string)this.ViewState["YearText"]) ?? "Year");
            }
            set
            {
                this.ViewState["YearText"] = value;
            }
        }
        #endregion
    }
}