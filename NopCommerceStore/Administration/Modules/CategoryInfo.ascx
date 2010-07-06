<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CategoryInfoControl"
    CodeBehind="CategoryInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SelectCategoryControl" Src="SelectCategoryControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register Assembly="NopCommerceStore" Namespace="NopSolutions.NopCommerce.Web.Controls"
    TagPrefix="nopCommerce" %>

<%if (this.HasLocalizableContent)
  { %>
<div id="localizablecontentpanel" class="tabcontainer-usual">
    <ul class="idTabs">
        <li class="idTab"><a href="#idTab_Info1" class="selected"><%=GetLocaleResourceString("Admin.Localizable.Standard")%></a></li>
        <asp:Repeater ID="rptrLanguageTabs" runat="server">
            <ItemTemplate>
                <li class="idTab"><a href="#idTab_Info<%# Container.ItemIndex+2 %>">
                    <asp:Image runat="server" ID="imgCFlag" Visible='<%# !String.IsNullOrEmpty(Eval("IconURL").ToString()) %>'
                        AlternateText='<%#Eval("Name")%>' ImageUrl='<%#Eval("IconURL").ToString()%>' />
                    <%#Server.HtmlEncode(Eval("Name").ToString())%></a></li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
    <div id="idTab_Info1" class="tab">
        <%} %>
        <table class="adminContent">
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblCategoryName" Text="<% $NopResources:Admin.CategoryInfo.Name %>"
                        ToolTip="<% $NopResources:Admin.CategoryInfo.Name.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtName" ErrorMessage="<% $NopResources:Admin.CategoryInfo.Name.Required %>">
                    </nopCommerce:SimpleTextBox>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblCategoryDescription" Text="<% $NopResources:Admin.CategoryInfo.Description %>"
                        ToolTip="<% $NopResources:Admin.CategoryInfo.Description.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <nopCommerce:NopHTMLEditor ID="txtDescription" runat="server" Height="350" />
                </td>
            </tr>
        </table>
        <%if (this.HasLocalizableContent)
          { %></div>
    <asp:Repeater ID="rptrLanguageDivs" runat="server" OnItemDataBound="rptrLanguageDivs_ItemDataBound">
        <ItemTemplate>
            <div id="idTab_Info<%# Container.ItemIndex+2 %>" class="tab">
                 <i><%=GetLocaleResourceString("Admin.Localizable.EmptyFieldNote")%></i>
                <asp:Label ID="lblLanguageId" runat="server" Text='<%#Eval("LanguageId") %>' Visible="false"></asp:Label>
                <table class="adminContent">
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedCategoryName" Text="<% $NopResources:Admin.CategoryInfo.Name %>"
                                ToolTip="<% $NopResources:Admin.CategoryInfo.Name.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" CssClass="adminInput" ID="txtLocalizedCategoryName">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedCategoryDescription" Text="<% $NopResources:Admin.CategoryInfo.Description %>"
                                ToolTip="<% $NopResources:Admin.CategoryInfo.Description.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:NopHTMLEditor ID="txtLocalizedDescription" runat="server" Height="350" />
                        </td>
                    </tr>
                </table>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
<%} %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCategoryImage" Text="<% $NopResources:Admin.CategoryInfo.Image %>"
                ToolTip="<% $NopResources:Admin.CategoryInfo.Image.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Image ID="iCategoryPicture" runat="server" AlternateText="category image" />
            <br />
            <asp:Button ID="btnRemoveCategoryImage" CssClass="adminInput" CausesValidation="false"
                runat="server" Text="<% $NopResources:Admin.CategoryInfo.Image.Remove %>" OnClick="btnRemoveCategoryImage_Click"
                Visible="false" ToolTip="<% $NopResources:Admin.CategoryInfo.Image.Remove.Tooltip %>" />
            <br />
            <asp:FileUpload ID="fuCategoryPicture" CssClass="adminInput" runat="server" ToolTip="<% $NopResources:Admin.CategoryInfo.Image.Upload %>" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCategoryTemplate" Text="<% $NopResources:Admin.CategoryInfo.CategoryTemplate %>"
                ToolTip="<% $NopResources: Admin.CategoryInfo.CategoryTemplate.Tooltip%>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlTemplate" AutoPostBack="False" CssClass="adminInput" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblParentCategory" Text="<% $NopResources:Admin.CategoryInfo.ParentCategory %>"
                ToolTip="<% $NopResources:Admin.CategoryInfo.ParentCategory.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SelectCategoryControl ID="ParentCategory" CssClass="adminInput" runat="server">
            </nopCommerce:SelectCategoryControl>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCategoryPriceRanges" Text="<% $NopResources: Admin.CategoryInfo.PriceRanges%>"
                ToolTip="<% $NopResources: Admin.CategoryInfo.PriceRanges.Tooltip%>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtPriceRanges" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCategoryShowOnHomePage" Text="<% $NopResources:Admin.CategoryInfo.ShowOnHomePage %>"
                ToolTip="<% $NopResources:Admin.CategoryInfo.ShowOnHomePage.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbShowOnHomePage" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCategoryPublished" Text="<% $NopResources:Admin.CategoryInfo.Published %>"
                ToolTip="<% $NopResources:Admin.CategoryInfo.Published.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbPublished" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCategoryDisplayOrder" Text="<% $NopResources:Admin.CategoryInfo.DisplayOrder %>"
                ToolTip="<% $NopResources:Admin.CategoryInfo.DisplayOrder.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" ID="txtDisplayOrder" CssClass="adminInput"
                Value="1" RequiredErrorMessage="<% $NopResources:Admin.CategoryInfo.DisplayOrder.RequiredErrorMessage %>"
                RangeErrorMessage="<% $NopResources:Admin.CategoryInfo.DisplayOrder.RangeErrorMessage %>"
                MinimumValue="-99999" MaximumValue="99999"></nopCommerce:NumericTextBox>
        </td>
    </tr>
</table>
