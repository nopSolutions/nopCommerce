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
// Contributor(s): Christoffer Munck. 
//------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace NopSolutions.NopCommerce.Web.Controls
{
    public class Lite : AjaxControlToolkit.HTMLEditor.Editor
    {
        protected override void FillBottomToolbar()
        {
            BottomToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.DesignMode());
            BottomToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HtmlMode());
            BottomToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.PreviewMode());
        }

        protected override void FillTopToolbar()
        {
            Collection<AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption> options;
            AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption option;

            TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.Bold());
            TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.Italic());
            TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.Underline());
            TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());

            AjaxControlToolkit.HTMLEditor.ToolbarButton.FixedForeColor FixedForeColor = new AjaxControlToolkit.HTMLEditor.ToolbarButton.FixedForeColor();
            TopToolbar.Buttons.Add(FixedForeColor);
            AjaxControlToolkit.HTMLEditor.ToolbarButton.ForeColorSelector ForeColorSelector = new AjaxControlToolkit.HTMLEditor.ToolbarButton.ForeColorSelector();
            ForeColorSelector.FixedColorButtonId = FixedForeColor.ID = "FixedForeColor";
            TopToolbar.Buttons.Add(ForeColorSelector);
            TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.ForeColorClear());
            TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());

            AjaxControlToolkit.HTMLEditor.ToolbarButton.FontName fontName = new AjaxControlToolkit.HTMLEditor.ToolbarButton.FontName();
            TopToolbar.Buttons.Add(fontName);

            options = fontName.Options;
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Value = "arial,helvetica,sans-serif";
            option.Text = "Arial";
            options.Add(option);
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Value = "courier new,courier,monospace";
            option.Text = "Courier New";
            options.Add(option);
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Value = "georgia,times new roman,times,serif";
            option.Text = "Georgia";
            options.Add(option);
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Value = "tahoma,arial,helvetica,sans-serif";
            option.Text = "Tahoma";
            options.Add(option);
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Value = "times new roman,times,serif";
            option.Text = "Times New Roman";
            options.Add(option);
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Value = "verdana,arial,helvetica,sans-serif";
            option.Text = "Verdana";
            options.Add(option);
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Value = "impact";
            option.Text = "Impact";
            options.Add(option);
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Value = "wingdings";
            option.Text = "WingDings";
            options.Add(option);

            TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());
            AjaxControlToolkit.HTMLEditor.ToolbarButton.FontSize fontSize = new AjaxControlToolkit.HTMLEditor.ToolbarButton.FontSize();
            TopToolbar.Buttons.Add(fontSize);

            options = fontSize.Options;
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Value = "8pt";
            option.Text = "1 ( 8 pt)";
            options.Add(option);
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Value = "10pt";
            option.Text = "2 (10 pt)";
            options.Add(option);
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Value = "12pt";
            option.Text = "3 (12 pt)";
            options.Add(option);
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Value = "14pt";
            option.Text = "4 (14 pt)";
            options.Add(option);
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Value = "18pt";
            option.Text = "5 (18 pt)";
            options.Add(option);
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Value = "24pt";
            option.Text = "6 (24 pt)";
            options.Add(option);
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Value = "36pt";
            option.Text = "7 (36 pt)";
            options.Add(option);

            TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());
            TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.RemoveStyles());
        }
    }

    public class LiteNoBottom : Lite
    {
        protected override void FillBottomToolbar()
        {
        }
    }

    public class FullNoBottom : AjaxControlToolkit.HTMLEditor.Editor
    {
        protected override void FillBottomToolbar()
        {
        }
    }

    public class FullWithRightBottom : AjaxControlToolkit.HTMLEditor.Editor
    {
        protected override void FillBottomToolbar()
        {
            // reverse
            BottomToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.PreviewMode());
            BottomToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HtmlMode());
            BottomToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.DesignMode());
        }
    }

    public class NopHTMLEditor : AjaxControlToolkit.HTMLEditor.Editor
    {
        protected override void FillTopToolbar()
        {
            base.FillTopToolbar();
            TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());
            TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.CustomToolbarButton.InsertDate());
            //TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.CustomToolbarButton.InsertIcon());
            TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.CustomToolbarButton.InsertPicture());
        }

        public override string ButtonImagesFolder
        {
            get
            {
                return "~/editors/HTMLEditor/HTMLEditorCustomButtons/";
            }
        }
    }
}