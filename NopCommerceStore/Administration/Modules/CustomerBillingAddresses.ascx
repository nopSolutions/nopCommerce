<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerBillingAddressesControl"
    CodeBehind="CustomerBillingAddresses.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="AddressDisplay" Src="AddressDisplay.ascx" %>
<asp:Button runat="server" ID="AddBillingAddress" CssClass="adminButton" OnClick="AddBillingAddress_Click"
    Text="<% $NopResources:Admin.CustomerBillingAddresses.AddButton.Text %>" ToolTip="<% $NopResources:Admin.CustomerBillingAddresses.AddButton.Tooltip %>" />
<asp:DataList ID="dlBillingAddresses" runat="server" RepeatColumns="3" RepeatDirection="Horizontal"
    Width="100%">
    <ItemTemplate>
        <table class="adminContent">
            <tr>
                <td>
                    <nopCommerce:AddressDisplay ID="AddressDisplayCtrl" runat="server" Address='<%# Container.DataItem %>' />
                </td>
            </tr>
            <tr>
                <td>
                    <a href="AddressDetails.aspx?AddressID=<%#Eval("AddressId")%>">
                        <%#GetLocaleResourceString("Admin.CustomerBillingAddresses.Edit")%>
                    </a>
                </td>
            </tr>
        </table>
    </ItemTemplate>
</asp:DataList>