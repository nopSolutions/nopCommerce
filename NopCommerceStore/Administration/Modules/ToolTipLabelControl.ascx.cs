using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ToolTipLabelControl : BaseNopAdministrationUserControl
    {
        public string Text
        {
            get
            {
                return lblValue.Text;
            }
            set
            {
                lblValue.Text = value;
            }
        }

        public string ToolTip
        {
            get
            {
                return lblValue.ToolTip;
            }
            set
            {
                lblValue.ToolTip = value;
                imgToolTip.ToolTip = value;
            }
        }

        public string ToolTipImage
        {
            get
            {
                return imgToolTip.ImageUrl;
            }
            set
            {
                imgToolTip.ImageUrl = value;
            }
        }


        public Unit Width
        {
            get
            {
                return lblValue.Width;
            }
            set
            {
                lblValue.Width = value;
            }
        }

        public string CssClass
        {
            get
            {
                return lblValue.CssClass;
            }
            set
            {
                lblValue.CssClass = value;
            }
        }



    }
}