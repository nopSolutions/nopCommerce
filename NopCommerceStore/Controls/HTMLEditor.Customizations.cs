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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;
using System.CodeDom;
using System.Drawing;
using System.IO;
using AjaxControlToolkit;
using AjaxControlToolkit.HTMLEditor;
using AjaxControlToolkit.HTMLEditor.ToolbarButton;

[assembly: WebResource("editors.HTMLEditor.HTMLEditorScripts.InsertDate.js", "application/x-javascript")]
namespace AjaxControlToolkit.HTMLEditor.CustomToolbarButton
{
    [ParseChildren(true)]
    [PersistChildren(false)]
    [RequiredScript(typeof(OkCancelPopupButton))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
    public class InsertIcon : DesignModePopupImageButton
    {

        #region [ Properties ]

        [DefaultValue(10)]
        [Category("Appearance")]
        [Description("Icons in one row of the ralated popup")]
        public int IconsInRow
        {
            get { return (int)(ViewState["IconsInRow"] ?? 10); }
            set { ViewState["IconsInRow"] = value; }
        }

        [DefaultValue("~/editors/HTMLEditor/HTMLEditorIcons/")]
        [Category("Appearance")]
        [Description("Folder used for icons")]
        public string IconsFolder
        {
            get { return (string)(ViewState["IconsFolder"] ?? "~/editors/HTMLEditor/HTMLEditorIcons/"); }
            set { ViewState["IconsFolder"] = value; }
        }

        #endregion

        #region [ Methods ]

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            RelatedPopup = new CustomPopups.InsertIconPopup();
            (RelatedPopup as CustomPopups.InsertIconPopup).IconsInRow = IconsInRow;
            (RelatedPopup as CustomPopups.InsertIconPopup).IconsFolder = IconsFolder;
        }

        protected override void OnPreRender(EventArgs e)
        {
            RegisterButtonImages("ed_insertIcon");
            base.OnPreRender(e);
        }

        protected override string ClientControlType
        {
            get { return "Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertIcon"; }
        }

        public override string ScriptPath
        {
            get { return "~/editors/HTMLEditor/HTMLEditorScripts/InsertIcon.js"; }
        }

        public override string ToolTip
        {
            get { return "Insert predefined icon"; }
        }

        #endregion
    }

    [ParseChildren(true)]
    [PersistChildren(false)]
    [RequiredScript(typeof(MethodButton))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
    public class InsertPicture : MethodButton
    {
        #region [ Methods ]

        protected override void OnPreRender(EventArgs e)
        {
            RegisterButtonImages("ed_insertPicture");
            base.OnPreRender(e);
        }

        protected override string ClientControlType
        {
            get { return "Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertPicture"; }
        }

        public override string ScriptPath
        {
            get { return "~/editors/HTMLEditor/HTMLEditorScripts/InsertPicture.js"; }
        }

        public override string ToolTip
        {
            get { return "Open a picture browser"; }
        }

        #endregion
    }



    [ParseChildren(true)]
    [PersistChildren(false)]
    [RequiredScript(typeof(MethodButton))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
    public class InsertDate : MethodButton
    {
        #region [ Methods ]

        protected override void OnPreRender(EventArgs e)
        {
            RegisterButtonImages("ed_date");
            base.OnPreRender(e);
        }

        protected override string ClientControlType
        {
            get { return "Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertDate"; }
        }

        public override string ScriptPath
        {
            get { return "~/editors/HTMLEditor/HTMLEditorScripts/InsertDate.js"; }
        }

        public override string ToolTip
        {
            get { return "Insert current date"; }
        }

        #endregion
    }
}

namespace AjaxControlToolkit.HTMLEditor.CustomPopups
{
    [ParseChildren(true)]
    [RequiredScript(typeof(Popups.AttachedTemplatePopup))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
    internal class InsertIconPopup : Popups.AttachedTemplatePopup
    {
        #region [ Properties ]

        [DefaultValue(10)]
        [Category("Appearance")]
        [Description("Icons in one row")]
        public int IconsInRow
        {
            get { return (int)(ViewState["IconsInRow"] ?? 10); }
            set { ViewState["IconsInRow"] = value; }
        }

        [DefaultValue("~/editors/HTMLEditor/HTMLEditorIcons/")]
        [Category("Appearance")]
        [Description("Folder used for icons")]
        public string IconsFolder
        {
            get { return (string)(ViewState["IconsFolder"] ?? "~/editors/HTMLEditor/HTMLEditorIcons/"); }
            set { ViewState["IconsFolder"] = value; }
        }

        #endregion

        #region [ Methods ]

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        protected string LocalResolveUrl(string path)
        {
            string temp = base.ResolveUrl(path);
            Regex _Regex = new Regex(@"(\(S\([A-Za-z0-9_]+\)\)/)", RegexOptions.Compiled);
            temp = _Regex.Replace(temp, "");
            return temp;
        }

        protected override void CreateChildControls()
        {
            Table table = new Table();
            TableRow row = null;
            TableCell cell;

            string iconsFolder = LocalResolveUrl(this.IconsFolder);
            if (iconsFolder.Length > 0)
            {
                string lastCh = iconsFolder.Substring(iconsFolder.Length - 1, 1);
                if (lastCh != "\\" && lastCh != "/") iconsFolder += "/";
            }

            if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(iconsFolder)))
            {
                string[] files = Directory.GetFiles(System.Web.HttpContext.Current.Server.MapPath(iconsFolder));
                int j = 0;

                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext == ".gif" || ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                    {
                        if (j == 0)
                        {
                            row = new TableRow();
                            table.Rows.Add(row);
                        }
                        cell = new TableCell();
                        System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
                        image.ImageUrl = iconsFolder + Path.GetFileName(file);
                        image.Attributes.Add("onmousedown", "insertImage(\"" + iconsFolder + Path.GetFileName(file) + "\")");
                        image.Style[HtmlTextWriterStyle.Cursor] = "pointer";
                        cell.Controls.Add(image);
                        row.Cells.Add(cell);

                        j++;
                        if (j == IconsInRow) j = 0;
                    }
                }
            }
            table.Attributes.Add("border", "0");
            table.Attributes.Add("cellspacing", "2");
            table.Attributes.Add("cellpadding", "0");
            table.Style["background-color"] = "transparent";

            Content.Add(table);

            base.CreateChildControls();
        }

        #endregion
    }
}