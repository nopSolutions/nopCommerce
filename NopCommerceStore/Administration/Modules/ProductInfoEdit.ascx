<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductInfoControl"
    CodeBehind="ProductInfoEdit.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register Assembly="NopCommerceStore" Namespace="NopSolutions.NopCommerce.Web.Controls"
    TagPrefix="nopCommerce" %>

<%if (this.HasLocalizableContent)
  { %>
<div id="localizablecontentpanel" class="tabcontainer-usual">
    <ul class="idTabs">
        <li class="idTab"><a href="#idTab_Info1" class="selected">
            <%=GetLocaleResourceString("Admin.Localizable.Standard")%></a></li>
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
                    <nopCommerce:ToolTipLabel runat="server" ID="lblProductName" Text="<% $NopResources:Admin.ProductInfo.ProductName %>"
                        ToolTip="<% $NopResources:Admin.ProductInfo.ProductName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.ProductInfo.ProductName.ErrorMessage %>">
                    </nopCommerce:SimpleTextBox>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblShortDescription" Text="<% $NopResources:Admin.ProductInfo.ShortDescription %>"
                        ToolTip="<% $NopResources:Admin.ProductInfo.ShortDescription.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <asp:TextBox ID="txtShortDescription" runat="server" CssClass="adminInput" TextMode="MultiLine"
                        Height="100"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblFullDescription" Text="<% $NopResources:Admin.ProductInfo.FullDescription %>"
                        ToolTip="<% $NopResources: Admin.ProductInfo.FullDescription.Tooltip%>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <nopCommerce:NopHTMLEditor ID="txtFullDescription" runat="server" Height="350" />
                </td>
            </tr>
        </table>
        <%if (this.HasLocalizableContent)
          { %></div>
    <asp:Repeater ID="rptrLanguageDivs" runat="server" OnItemDataBound="rptrLanguageDivs_ItemDataBound">
        <ItemTemplate>
            <div id="idTab_Info<%# Container.ItemIndex+2 %>" class="tab">
                <i>
                    <%=GetLocaleResourceString("Admin.Localizable.EmptyFieldNote")%></i>
                <asp:Label ID="lblLanguageId" runat="server" Text='<%#Eval("LanguageId") %>' Visible="false"></asp:Label>
                <table class="adminContent">
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedProductName" Text="<% $NopResources:Admin.ProductInfo.ProductName %>"
                                ToolTip="<% $NopResources:Admin.ProductInfo.ProductName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" ID="txtLocalizedName" CssClass="adminInput">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedShortDescription" Text="<% $NopResources:Admin.ProductInfo.ShortDescription %>"
                                ToolTip="<% $NopResources:Admin.ProductInfo.ShortDescription.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtLocalizedShortDescription" runat="server" CssClass="adminInput"
                                TextMode="MultiLine" Height="100"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedFullDescription" Text="<% $NopResources:Admin.ProductInfo.FullDescription %>"
                                ToolTip="<% $NopResources: Admin.ProductInfo.FullDescription.Tooltip%>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:NopHTMLEditor ID="txtLocalizedFullDescription" runat="server" Height="350" />
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
            <nopCommerce:ToolTipLabel runat="server" ID="lblAdminComment" Text="<% $NopResources:Admin.ProductInfo.AdminComment %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.AdminComment.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtAdminComment" runat="server" CssClass="adminInput" TextMode="MultiLine"
                Height="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblProductTemplate" Text="<% $NopResources:Admin.ProductInfo.ProductTemplate %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.ProductTemplate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlTemplate" AutoPostBack="False" CssClass="adminInput" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblShowOnHomePage" Text="<% $NopResources:Admin.ProductInfo.ShowOnHomePage %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.ShowOnHomePage.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbShowOnHomePage" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblProductPublished" Text="<% $NopResources:Admin.ProductInfo.Published %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.Published.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbPublished" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAllowCustomerReviews" Text="<% $NopResources:Admin.ProductInfo.AllowCustomerReviews %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.AllowCustomerReviews.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbAllowCustomerReviews" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:HyperLink ID="hlViewReviews" runat="server" Text="View reviews" ToolTip="<% $NopResources:Admin.ProductInfo.AllowCustomerReviewsView.Tooltip %>" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAllowCustomerRatings" Text="<% $NopResources:Admin.ProductInfo.AllowCustomerRatings %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.AllowCustomerRatings.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbAllowCustomerRatings" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblProductTags" Text="<% $NopResources:Admin.ProductInfo.ProductTags %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.ProductTags.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtProductTags" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
</table>
