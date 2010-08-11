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
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.ExportImport;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CustomersControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SetDefaultValues();
                phUsername.Visible = CustomerManager.UsernamesEnabled;
                phDateOfBirth.Visible = CustomerManager.FormFieldDateOfBirthEnabled;
                gvCustomers.Columns[2].Visible = CustomerManager.UsernamesEnabled;

                //buttons
                btnExportXLS.Visible = SettingManager.GetSettingValueBoolean("Features.SupportExcel");
                btnImportXLS.Visible = SettingManager.GetSettingValueBoolean("Features.SupportExcel");
            }
        }

        protected void SetDefaultValues()
        {
            int days = CommonHelper.QueryStringInt("ShowDays");
            if (days > 0)
            {
                ctrlStartDatePicker.SelectedDate = DateTime.UtcNow.AddDays(-days);
            }
        }

        protected List<Customer> GetCustomers()
        {
            DateTime? startDate = ctrlStartDatePicker.SelectedDate;
            DateTime? endDate = ctrlEndDatePicker.SelectedDate;
            if(startDate.HasValue)
            {
                startDate = DateTimeHelper.ConvertToUtcTime(startDate.Value, DateTimeHelper.CurrentTimeZone);
            }
            if(endDate.HasValue)
            {
                endDate = DateTimeHelper.ConvertToUtcTime(endDate.Value, DateTimeHelper.CurrentTimeZone).AddDays(1);
            }

            string email = txtEmail.Text.Trim();
            string username = txtUsername.Text.Trim();
            bool dontLoadGuestCustomers = cbDontLoadGuestCustomers.Checked;
            int dateOfBirthDay = int.Parse(this.ddlDateOfBirthDay.SelectedValue);
            int dateOfBirthMonth = int.Parse(this.ddlDateOfBirthMonth.SelectedValue);
            int totalRecords = 0;
            var customers = CustomerManager.GetAllCustomers(startDate,
                endDate, email, username, dontLoadGuestCustomers,
                dateOfBirthMonth, dateOfBirthDay, int.MaxValue, 0, out totalRecords);
            return customers;
        }

        protected void BindGrid()
        {
            var customers = GetCustomers();
            gvCustomers.DataSource = customers;
            gvCustomers.DataBind();
        }

        protected string GetCustomerInfo(Customer customer)
        {
            string customerInfo = string.Empty;
            if (customer != null)
            {
                if (customer.IsGuest)
                {
                    customerInfo = Server.HtmlEncode(GetLocaleResourceString("Admin.Customers.Guest"));
                }
                else
                {
                    customerInfo = Server.HtmlEncode(customer.Email);
                }
            }
            return customerInfo;
        }

        protected void gvCustomers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCustomers.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    BindGrid();
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void btnExportXML_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    string fileName = string.Format("customers_{0}.xml", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

                    var customers = GetCustomers();
                    string xml = ExportManager.ExportCustomersToXml(customers);
                    CommonHelper.WriteResponseXml(xml, fileName);
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void btnExportXLS_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    string fileName = string.Format("customers_{0}_{1}.xls", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));
                    string filePath = string.Format("{0}files\\ExportImport\\{1}", HttpContext.Current.Request.PhysicalApplicationPath, fileName);
                    var customers = GetCustomers();

                    ExportManager.ExportCustomersToXls(filePath, customers);
                    CommonHelper.WriteResponseXls(filePath, fileName);
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void btnImportXLS_Click(object sender, EventArgs e)
        {
            if (fuXlsFile.PostedFile != null && !String.IsNullOrEmpty(fuXlsFile.FileName))
            {
                try
                {
                    byte[] fileBytes = fuXlsFile.FileBytes;
                    string extension = "xls";
                    if (fuXlsFile.FileName.EndsWith("xlsx"))
                        extension = "xlsx";
                    
                    string fileName = string.Format("customers_{0}_{1}.{2}", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4), extension);
                    string filePath = string.Format("{0}files\\ExportImport\\{1}", HttpContext.Current.Request.PhysicalApplicationPath, fileName);

                    File.WriteAllBytes(filePath, fileBytes);
                    ImportManager.ImportCustomersFromXls(filePath);

                    BindGrid();
                }
                catch (Exception ex)
                {
                    ProcessException(ex);
                }
            }
        }
    }
}