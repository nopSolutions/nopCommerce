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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class DatePicker2Control : BaseNopUserControl
    {
        protected override void  OnInit(EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                lstDays.Items.Add(new ListItem(GetLocaleResourceString("DatePicker2.Day"), "0"));
                for(int i = 1; i <= 31; i++)
                {
                    lstDays.Items.Add(new ListItem(i.ToString("00"), i.ToString()));
                }

                lstMonths.Items.Add(new ListItem(GetLocaleResourceString("DatePicker2.Month"), "0"));
                for(int i = 1; i <= 12; i++)
                {
                    lstMonths.Items.Add(new ListItem(i.ToString("00"), i.ToString()));
                }

                lstYears.Items.Add(new ListItem(GetLocaleResourceString("DatePicker2.Year"), "0"));
                int startYear = DateTime.Now.Year;
                for(int i = startYear; i >= startYear - 110; i--)
                {
                    lstYears.Items.Add(i.ToString());
                }
            }
            base.OnInit(e);
        }

        public DateTime? SelectedDate
        {
            get
            {
                if(lstDays.SelectedIndex <= 0 || lstMonths.SelectedIndex <= 0 || lstYears.SelectedIndex <= 0)
                {
                    return null;
                }
                try
                {
                    return new DateTime(Int32.Parse(lstYears.SelectedValue), Int32.Parse(lstMonths.SelectedValue), Int32.Parse(lstDays.SelectedValue));
                }
                catch(Exception)
                {
                    return null;
                }
            }
            set
            {
                if(value.HasValue)
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
    }
}