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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.Common.Utils;


namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class PictureBrowserControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            gvPictures.PageSize = PageSize;

            if (!IsPostBack)
            {
                BindDropDownPaging();
                BindGrid();
                this.PictureTabs.Tabs[1].Visible = false;

                if (PictureID > 0)
                {
                    BindSizeRepeater(PictureID);
                    this.PictureTabs.Tabs[1].Visible = true;
                    this.SelectTab(this.PictureTabs, "pnlPictureDetails");
                }
            }
        }

        [DefaultValue(10)]
        protected int PageSize
        {
            get
            {
                object val = ViewState["PageSize"];

                if (val != null)
                {
                    return int.Parse(val.ToString());
                }

                return 10;

            }
            set
            {
                ViewState["PageSize"] = value;
            }
        }


        private void BindDropDownPaging()
        {
            if (ddlPageSize.Items.Count == 0)
            {

                for (int i = 10; i <= 100; i += 10)
                {

                    ListItem listItem = new ListItem(i.ToString(), i.ToString());
                    if (i == PageSize)
                    {
                        listItem.Selected = true;
                    }

                    ddlPageSize.Items.Add(listItem);
                }
            }

        }

        private void BindGrid()
        {
            int totalRecords = 0;
            var pictures = PictureManager.GetPictures(int.MaxValue, 0, out totalRecords);
            gvPictures.DataSource = pictures;
            gvPictures.DataBind();
        }

        private void BindSizeRepeater(int pictureID)
        {
            List<String> urls = PictureManager.GetPictureUrls(pictureID);
            repeaterPictureSizes.DataSource = urls;
            repeaterPictureSizes.DataBind();
        }

        protected void gvPictures_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPictures.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvPictures_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Picture picture = (Picture)e.Row.DataItem;

            if (picture != null)
            {
                Image imagePicture = e.Row.FindControl("imagePicture") as Image;
                if (imagePicture != null)
                    imagePicture.ImageUrl = PictureManager.GetPictureUrl(picture, 100);

            }
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSize = int.Parse(((DropDownList)sender).SelectedValue);
            this.PageSize = pageSize;
            gvPictures.PageSize = pageSize;

            BindGrid();
        }

        protected void cmdGenerateNewSize_Click(object sender, EventArgs e)
        {
            int width = txtNewPictureSize.Value;
            
            if (width > 10 && PictureID > 0)
            {
                PictureManager.GetPictureUrl(PictureID, width);
                BindSizeRepeater(PictureID);

            }
        }

        protected void cmdUpload_Click(object sender, EventArgs e)
        {
            HttpPostedFile categoryPictureFile = fuNewPicture.PostedFile;

            if (!string.IsNullOrEmpty(fuNewPicture.FileName))
            {
                byte[] categoryPictureBinary = PictureManager.GetPictureBits(categoryPictureFile.InputStream, categoryPictureFile.ContentLength);

                Picture picture = PictureManager.InsertPicture(categoryPictureBinary,
                    categoryPictureFile.ContentType, true);

                if (picture != null)
                {
                    string redirectURL = Common.Utils.CommonHelper.ModifyQueryString(Request.Url.PathAndQuery, "PictureID=" + picture.PictureId.ToString(), null);
                    Response.Redirect(redirectURL);
                }
            }
        }

        protected int PictureID
        {
            get
            {
                return Common.Utils.CommonHelper.QueryStringInt("PictureID", 0);
            }
        }
    }
}