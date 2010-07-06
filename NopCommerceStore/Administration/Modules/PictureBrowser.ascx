<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.PictureBrowserControl"
    CodeBehind="PictureBrowser.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>

<script type="text/javascript">
    function InsertToParent(url) {
        if (window.opener && !window.opener.closed) {
            window.opener.InsertPictureFromWindow(url);
            window.close();
        }

    }          
     
</script>

<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.PictureBrowser.Title")%>" />
        <%=GetLocaleResourceString("Admin.PictureBrowser.Title")%>
    </div>
</div>
<ajaxToolkit:TabContainer runat="server" ID="PictureTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlPictureBrowser" HeaderText="<% $NopResources: Admin.PictureBrowser.Pictures%>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblPageSize" ToolTipImage="~/Administration/Common/ico-help.gif"
                            Text="<% $NopResources:Admin.PictureBrowser.PageSize %>" ToolTip="<% $NopResources:Admin.PictureBrowser.PageSize.Tooltip %>" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <asp:GridView ID="gvPictures" runat="server" OnPageIndexChanging="gvPictures_PageIndexChanging"
                OnRowDataBound="gvPictures_RowDataBound" AllowPaging="True" DataKeyNames="PictureId"
                AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField DataField="PictureId" HeaderText="<%$ NopResources:Admin.PictureBrowser.PictureID %>"
                        ReadOnly="True" />
                    <asp:TemplateField HeaderText="<%$ NopResources:Admin.PictureBrowser.Picture %>">
                        <ItemTemplate>
                            <asp:Image ID="imagePicture" runat="server" AlternateText="pic" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$ NopResources:Admin.PictureBrowser.Details %>">
                        <ItemTemplate>
                            <a href='?PictureID=<%#Eval("PictureId") %>'>
                                <%=GetLocaleResourceString("Admin.PictureBrowser.Details")%></a>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlPictureDetails" HeaderText="<% $NopResources: Admin.PictureBrowser.Details%>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblGenerateAnothersize" Text="<% $NopResources:Admin.PictureBrowser.GenerateAnotherSize %>"
                            ToolTip="<% $NopResources:Admin.PictureBrowser.GenerateAnotherSize.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtNewPictureSize"
                            ValidationGroup="ImageSize" RequiredErrorMessage="<% $NopResources:Admin.PictureBrowser.GenerateAnotherSize.RequiredErrorMessage %>"
                            MinimumValue="30" MaximumValue="2000" Value="250" RangeErrorMessage="<% $NopResources:Admin.PictureBrowser.GenerateAnotherSize.RangeErrorMessage %>">
                        </nopCommerce:NumericTextBox>
                        (Width in pixels)<br />
                        <asp:Button ID="cmdGenerateNewSize" runat="server" OnClick="cmdGenerateNewSize_Click"
                            ValidationGroup="ImageSize" Text="<% $NopResources:Admin.PictureBrowser.GenerateAnotherSize %>" />
                    </td>
                </tr>
            </table>
            <asp:Repeater ID="repeaterPictureSizes" runat="server">
                <HeaderTemplate>
                    <table class="tablestyle" cellspacing="0">
                        <tr class="headerstyle">
                            <th>
                                <%=GetLocaleResourceString("Admin.PictureBrowser.Picture")%>
                            </th>
                            <th>
                                <%=GetLocaleResourceString("Admin.PictureBrowser.Insert")%>
                            </th>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="<%#(Container.ItemIndex % 2 == 0) ? "rowstyle" : "altrowstyle" %>">
                        <td>
                            <img src="<%# Container.DataItem %>" border="0" /><br />
                        </td>
                        <td>
                            <a href="javascript:InsertToParent('<%# Container.DataItem %>')">
                                <%=GetLocaleResourceString("Admin.PictureBrowser.Insert")%></a>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlPictureUpload" HeaderText="<% $NopResources: Admin.PictureBrowser.UploadNewPicture%>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblNewPicture" Text="<% $NopResources:Admin.PictureBrowser.UploadNewPicture %>"
                            ToolTip="<% $NopResources:Admin.PictureBrowser.UploadNewPicture.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:FileUpload ID="fuNewPicture" CssClass="adminInput" runat="server" ToolTip="<% $NopResources:Admin.PictureBrowser.FileUpload %>" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblUpload" Text="<% $NopResources:Admin.PictureBrowser.SaveNewPicture %>"
                            ToolTip="<% $NopResources:Admin.PictureBrowser.SaveNewPicture.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Button ID="cmdUpload" runat="server" Text="<% $NopResources:Admin.PictureBrowser.SaveNewPicture %>"
                            OnClick="cmdUpload_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
