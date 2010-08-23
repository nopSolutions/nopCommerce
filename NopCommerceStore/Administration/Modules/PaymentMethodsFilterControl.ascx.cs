using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using System.Text;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class PaymentMethodsFilterControl : BaseNopAdministrationUserControl
    {
        public class NopGridViewCustomTemplate : ITemplate
        {
            #region Fields
            private DataControlRowType _templateType;
            private string _columnName;
            private string _dataType;
            private int _paymentMethodId;
            #endregion

            #region Ctor
            public NopGridViewCustomTemplate(DataControlRowType type, string columnName, string DataType) : this(type, columnName, DataType, 0)
            {
            }

            public NopGridViewCustomTemplate(DataControlRowType type, string columnName, string dataType, int paymentMethodId)
            {
                _templateType = type;
                _columnName = columnName;
                _dataType = dataType;
                _paymentMethodId = paymentMethodId;
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
                        PaymentMethodCountryMappingHelperClass map1 = row.DataItem as PaymentMethodCountryMappingHelperClass;
                        PaymentMethod pm = PaymentMethodManager.GetPaymentMethodById(_paymentMethodId);
                        if (pm != null)
                        {
                            switch (pm.PaymentMethodType)
                            {
                                case PaymentMethodTypeEnum.Unknown:
                                case PaymentMethodTypeEnum.Standard:
                                    {
                                        (ctrl as CheckBox).Checked = map1.Restrict[_paymentMethodId];
                                    }
                                    break;
                                case PaymentMethodTypeEnum.Button:
                                    {
                                        (ctrl as CheckBox).Enabled = false;
                                        (ctrl as CheckBox).Text = "n/a";
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                }
            }

            private void hfCountryId_DataBinding(Object sender, EventArgs e)
            {
                HiddenField hf = sender as HiddenField;
                GridViewRow row = hf.NamingContainer as GridViewRow;

                PaymentMethodCountryMappingHelperClass map1 = row.DataItem as PaymentMethodCountryMappingHelperClass;
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
                        lc.Text = "<b>" + _columnName + "</b>&nbsp;";
                        container.Controls.Add(lc);
                        if(_paymentMethodId != 0)
                        {
                            CheckBox cbSelectAll = new CheckBox();
                            cbSelectAll.CssClass = String.Format("cbSelectAll_{0}", _paymentMethodId);
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
                                ctrl.ID = String.Format("cbRestrict_{0}", _paymentMethodId);
                                ctrl.CssClass = String.Format("cbRestrict_{0}", _paymentMethodId);
                                HiddenField hfCountryId = new HiddenField();
                                hfCountryId.ID = String.Format("hfCountryId_{0}", _paymentMethodId);
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

        protected class PaymentMethodCountryMappingHelperClass
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
            gvPaymentMethodCountryMap.Columns.Clear();

            TemplateField tfAction = new TemplateField();
            tfAction.ItemTemplate = new NopGridViewCustomTemplate(DataControlRowType.DataRow, "CountryName", "String");
            tfAction.HeaderTemplate = new NopGridViewCustomTemplate(DataControlRowType.Header, GetLocaleResourceString("Admin.PaymentMethodsFilterControl.Grid.CountryName"), "String");
            gvPaymentMethodCountryMap.Columns.Add(tfAction);

            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.Append("$(document).ready(function() {");
            foreach(PaymentMethod paymentMethod in PaymentMethodManager.GetAllPaymentMethods(null, false))
            {
                TemplateField tf = new TemplateField();
                tf.ItemTemplate = new NopGridViewCustomTemplate(DataControlRowType.DataRow, "Restrict", "Checkbox", paymentMethod.PaymentMethodId);
                tf.HeaderTemplate = new NopGridViewCustomTemplate(DataControlRowType.Header, paymentMethod.Name, "String", paymentMethod.PaymentMethodId);
                gvPaymentMethodCountryMap.Columns.Add(tf);
                
                scriptBuilder.AppendFormat("$('.cbSelectAll_{0} input').bind('click', function() {{ $('.cbRestrict_{0} input').each(function() {{ this.checked = $('.cbSelectAll_{0} input')[0].checked; }}) }});", paymentMethod.PaymentMethodId);
                //scriptBuilder.AppendFormat("$('.cbRestrict_{0} input').bind('click', function() {{ if (this.checked == false) $('.cbSelectAll_{0} input')[0].checked = false; }});", paymentMethod.PaymentMethodId);
            }
            scriptBuilder.Append("});");

            string script = scriptBuilder.ToString();
            Page.ClientScript.RegisterClientScriptBlock(GetType(), script.GetHashCode().ToString(), script, true);
        }

        protected void BindGrid()
        {
            var countryCollection = CountryManager.GetAllCountries();
            var paymentMethodCollection = PaymentMethodManager.GetAllPaymentMethods(null, false);

            if(countryCollection.Count == 0)
            {
                lblMessage.Text = GetLocaleResourceString("Admin.PaymentMethodsFilterControl.NoCountryDefined");
            }
            if(paymentMethodCollection.Count == 0)
            {
                lblMessage.Text = GetLocaleResourceString("Admin.PaymentMethodsFilterControl.NoPaymentMethodDefined");
            }

            List<PaymentMethodCountryMappingHelperClass> dt = new List<PaymentMethodCountryMappingHelperClass>();
            foreach(Country country in countryCollection)
            {
                PaymentMethodCountryMappingHelperClass map1 = new PaymentMethodCountryMappingHelperClass();
                map1.CountryId = country.CountryId;
                map1.CountryName = country.Name;
                map1.Restrict = new Dictionary<int, bool>();

                foreach(PaymentMethod paymentMethod in paymentMethodCollection)
                {
                    map1.Restrict.Add(paymentMethod.PaymentMethodId, PaymentMethodManager.DoesPaymentMethodCountryMappingExist(paymentMethod.PaymentMethodId, country.CountryId));
                }

                dt.Add(map1);
            }

            gvPaymentMethodCountryMap.DataSource = dt;
            gvPaymentMethodCountryMap.DataBind();
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
            var paymentMethods = PaymentMethodManager.GetAllPaymentMethods(null, false);
            foreach (GridViewRow row in gvPaymentMethodCountryMap.Rows)
            {
                foreach (PaymentMethod paymentMethod in paymentMethods)
                {
                    CheckBox cbRestrict = row.FindControl(String.Format("cbRestrict_{0}", paymentMethod.PaymentMethodId)) as CheckBox;
                    HiddenField hfCountryId = row.FindControl(String.Format("hfCountryId_{0}", paymentMethod.PaymentMethodId)) as HiddenField;
                    if (cbRestrict == null || hfCountryId == null)
                        return;

                    int countryId = Int32.Parse(hfCountryId.Value);

                    if (cbRestrict.Checked)
                    {
                        PaymentMethodManager.CreatePaymentMethodCountryMapping(paymentMethod.PaymentMethodId, countryId);
                    }
                    else
                    {
                        if (PaymentMethodManager.DoesPaymentMethodCountryMappingExist(paymentMethod.PaymentMethodId, countryId))
                        {
                            PaymentMethodManager.DeletePaymentMethodCountryMapping(paymentMethod.PaymentMethodId, countryId);
                        }
                    }
                }
            }
        }
    }
}