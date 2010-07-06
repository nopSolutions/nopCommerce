<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CheckoutAttributeInfoControl"
    CodeBehind="CheckoutAttributeInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
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
                    <nopCommerce:ToolTipLabel runat="server" ID="lblName" Text="<% $NopResources:Admin.CheckoutAttributeInfo.Name.Text %>"
                        ToolTip="<% $NopResources:Admin.CheckoutAttributeInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.CheckoutAttributeInfo.Name.ErrorMessage %>">
                    </nopCommerce:SimpleTextBox>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblTextPrompt" Text="<% $NopResources:Admin.CheckoutAttributeInfo.TextPrompt.Text %>"
                        ToolTip="<% $NopResources:Admin.CheckoutAttributeInfo.TextPrompt.Tooltip %>"
                        ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <asp:TextBox ID="txtTextPrompt" runat="server" CssClass="adminInput"></asp:TextBox>
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
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedName" Text="<% $NopResources:Admin.CheckoutAttributeInfo.Name.Text %>"
                                ToolTip="<% $NopResources:Admin.CheckoutAttributeInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" ID="txtLocalizedName" CssClass="adminInput">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedTextPrompt" Text="<% $NopResources:Admin.CheckoutAttributeInfo.TextPrompt.Text %>"
                                ToolTip="<% $NopResources:Admin.CheckoutAttributeInfo.TextPrompt.Tooltip %>"
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtLocalizedTextPrompt" runat="server" CssClass="adminInput"></asp:TextBox>
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
            <nopCommerce:ToolTipLabel runat="server" ID="lblAttributeRequired" Text="<% $NopResources:Admin.CheckoutAttributeInfo.Required %>"
                ToolTip="<% $NopResources:Admin.CheckoutAttributeInfo.Required.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbAttributeRequired" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblShippableProductRequired" Text="<% $NopResources:Admin.CheckoutAttributeInfo.ShippableProductRequired %>"
                ToolTip="<% $NopResources:Admin.CheckoutAttributeInfo.ShippableProductRequired.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbShippableProductRequired" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTaxExempt" Text="<% $NopResources:Admin.CheckoutAttributeInfo.TaxExempt %>"
                ToolTip="<% $NopResources:Admin.CheckoutAttributeInfo.TaxExempt.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsTaxExempt" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTaxCategory" Text="<% $NopResources:Admin.CheckoutAttributeInfo.TaxCategory %>"
                ToolTip="<% $NopResources:Admin.CheckoutAttributeInfo.TaxCategory.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlTaxCategory" AutoPostBack="False" CssClass="adminInput"
                runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAttributeControlType" Text="<% $NopResources:Admin.CheckoutAttributeInfo.ControlType %>"
                ToolTip="<% $NopResources:Admin.CheckoutAttributeInfo.ControlType.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList class="text" ID="ddlAttributeControlType" AutoPostBack="False"
                CssClass="adminInput" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.CheckoutAttributeInfo.DisplayOrder %>"
                ToolTip="<% $NopResources:Admin.CheckoutAttributeInfo.DisplayOrder.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtDisplayOrder"
                Value="1" RequiredErrorMessage="<% $NopResources:Admin.CheckoutAttributeInfo.DisplayOrder.RequiredErrorMessage %>"
                RangeErrorMessage="<% $NopResources:Admin.CheckoutAttributeInfo.DisplayOrder.RangeErrorMessage %>"
                MinimumValue="-99999" MaximumValue="99999">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
</table>
