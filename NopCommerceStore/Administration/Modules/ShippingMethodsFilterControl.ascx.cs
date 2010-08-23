using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using System.Text;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ShippingMethodsFilterControl : BaseNopAdministrationUserControl
    {
        public class NopGridViewCustomTemplate : ITemplate
        {
            #region Fields
            private DataControlRowType _templateType;
            private string _columnName;
            private string _dataType;
            private int _shippingMethodId;
            #endregion

            #region Ctor
            public NopGridViewCustomTemplate(DataControlRowType type, 
                string columnName, string DataType) : this(type, columnName, DataType, 0)
            {
            }

            public NopGridViewCustomTemplate(DataControlRowType type, 
                string columnName, string dataType, int shippingMethodId)
            {
                _templateType = type;
                _columnName = columnName;
                _dataType = dataType;
                _shippingMethodId = shippingMethodId;
            }
            #endregion

            #region Utilities
            private void ctrl_DataBinding(Object sender, EventArgs e)
            {
                WebControl ctrl = sender as WebControl;
                GridViewRow row = ctrl.NamingContainer as GridViewRow;

                switch(_dataType)
                {
                    case "String":
                        object RawValue = DataBinder.Eval(row.DataItem, _columnName);
                        (ctrl as Label).Text = RawValue.ToString();
                        break;

                    case "Checkbox":
                        ShippingMethodCountryMappingHelperClass map1 = row.DataItem as ShippingMethodCountryMappingHelperClass;
                        (ctrl as CheckBox).Checked = map1.Restrict[_shippingMethodId];
                        break;
                }
            }

            private void hfCountryId_DataBinding(Object sender, EventArgs e)
            {
                HiddenField hf = sender as HiddenField;
                GridViewRow row = hf.NamingContainer as GridViewRow;

                ShippingMethodCountryMappingHelperClass map1 = row.DataItem as ShippingMethodCountryMappingHelperClass;
                hf.Value = map1.CountryId.ToString();
            }
            #endregion

            #region Methods
            public void InstantiateIn(Control container)
            {
                switch(_templateType)
                {
                    case DataControlRowType.Header:
                        Literal lc = new Literal();
                        lc.Text = "<b>" + _columnName + "</b>";
                        container.Controls.Add(lc);
                        if(_shippingMethodId != 0)
                        {
                            CheckBox cbSelectAll = new CheckBox();
                            cbSelectAll.CssClass = String.Format("cbSelectAll_{0}", _shippingMethodId);
                            container.Controls.Add(cbSelectAll);
                        }
                        break;

                    case DataControlRowType.DataRow:
                        WebControl ctrl = null;
                        switch(_dataType)
                        {
                            case "String":
                                ctrl = new Label();
                                break;

                            case "Checkbox":
                                ctrl = new CheckBox();
                                ctrl.ID = String.Format("cbRestrict_{0}", _shippingMethodId);
                                ctrl.CssClass = String.Format("cbRestrict_{0}", _shippingMethodId);
                                HiddenField hfCountryId = new HiddenField();
                                hfCountryId.ID = String.Format("hfCountryId_{0}", _shippingMethodId);
                                hfCountryId.DataBinding += new EventHandler(hfCountryId_DataBinding);
                                container.Controls.Add(hfCountryId);
                                break;

                            default:
                                throw new Exception("Not supported column type");
                        }
                        ctrl.DataBinding += new EventHandler(this.ctrl_DataBinding);
                        container.Controls.Add(ctrl);
                        break;
                }
            }
            #endregion
        }

        protected class ShippingMethodCountryMappingHelperClass
        {
            public int CountryId 
            { 
                get;
                set; 
            }

            public string CountryName 
            { 
                get; 
                set; 
            }

            public Dictionary<int, bool> Restrict 
            { 
                get; 
                set; 
            }
        }

        protected void BuildColumnsDynamically()
        {
            gvShippingMethodCountryMap.Columns.Clear();

            TemplateField tfAction = new TemplateField();
            tfAction.ItemTemplate = new NopGridViewCustomTemplate(DataControlRowType.DataRow, "CountryName", "String");
            tfAction.HeaderTemplate = new NopGridViewCustomTemplate(DataControlRowType.Header, GetLocaleResourceString("Admin.ShippingMethodsFilterControl.Grid.CountryName"), "String");
            gvShippingMethodCountryMap.Columns.Add(tfAction);

            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.Append("$(document).ready(function() {");
            foreach(ShippingMethod shippingMethod in ShippingMethodManager.GetAllShippingMethods())
            {
                TemplateField tf = new TemplateField();
                tf.ItemTemplate = new NopGridViewCustomTemplate(DataControlRowType.DataRow, "Restrict", "Checkbox", shippingMethod.ShippingMethodId);
                tf.HeaderTemplate = new NopGridViewCustomTemplate(DataControlRowType.Header, shippingMethod.Name, "String", shippingMethod.ShippingMethodId);
                gvShippingMethodCountryMap.Columns.Add(tf);

                scriptBuilder.AppendFormat("$('.cbSelectAll_{0} input').bind('click', function() {{ $('.cbRestrict_{0} input').each(function() {{ this.checked = $('.cbSelectAll_{0} input')[0].checked; }}) }});", shippingMethod.ShippingMethodId);
                //scriptBuilder.AppendFormat("$('.cbRestrict_{0} input').bind('click', function() {{ if (this.checked == false) $('.cbSelectAll_{0} input')[0].checked = false; }});", shippingMethod.ShippingMethodId);
            }
            scriptBuilder.Append("});");

            string script = scriptBuilder.ToString();
            Page.ClientScript.RegisterClientScriptBlock(GetType(), script.GetHashCode().ToString(), script, true);
        }

        protected void BindGrid()
        {
            var countryCollection = CountryManager.GetAllCountries();
            var shippingMethodCollection = ShippingMethodManager.GetAllShippingMethods();

            if(countryCollection.Count == 0)
            {
                lblMessage.Text = GetLocaleResourceString("Admin.ShippingMethodsFilterControl.NoCountryDefined");
            }
            if(shippingMethodCollection.Count == 0)
            {
                lblMessage.Text = GetLocaleResourceString("Admin.ShippingMethodsFilterControl.NoShippingMethodDefined");
            }

            List<ShippingMethodCountryMappingHelperClass> dt = new List<ShippingMethodCountryMappingHelperClass>();
            foreach(Country country in countryCollection)
            {
                ShippingMethodCountryMappingHelperClass map1 = new ShippingMethodCountryMappingHelperClass();
                map1.CountryId = country.CountryId;
                map1.CountryName = country.Name;
                map1.Restrict = new Dictionary<int, bool>();

                foreach(ShippingMethod shippingMethod in shippingMethodCollection)
                {
                    map1.Restrict.Add(shippingMethod.ShippingMethodId, ShippingMethodManager.DoesShippingMethodCountryMappingExist(shippingMethod.ShippingMethodId, country.CountryId));
                }

                dt.Add(map1);
            }

            gvShippingMethodCountryMap.DataSource = dt;
            gvShippingMethodCountryMap.DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            BindJQuery();
            BuildColumnsDynamically();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                BindGrid();
            }
        }

        public void SaveInfo()
        {
            var shippingMethods = ShippingMethodManager.GetAllShippingMethods();
            foreach (GridViewRow row in gvShippingMethodCountryMap.Rows)
            {
                foreach (ShippingMethod shippingMethod in shippingMethods)
                {
                    CheckBox cbRestrict = row.FindControl(String.Format("cbRestrict_{0}", shippingMethod.ShippingMethodId)) as CheckBox;
                    HiddenField hfCountryId = row.FindControl(String.Format("hfCountryId_{0}", shippingMethod.ShippingMethodId)) as HiddenField;
                    if (cbRestrict == null || hfCountryId == null)
                        return;

                    int countryId = Int32.Parse(hfCountryId.Value);

                    if (cbRestrict.Checked)
                    {
                        ShippingMethodManager.CreateShippingMethodCountryMapping(shippingMethod.ShippingMethodId, countryId);
                    }
                    else
                    {
                        if (ShippingMethodManager.DoesShippingMethodCountryMappingExist(shippingMethod.ShippingMethodId, countryId))
                        {
                            ShippingMethodManager.DeleteShippingMethodCountryMapping(shippingMethod.ShippingMethodId, countryId);
                        }
                    }
                }
            }
        }
    }
}