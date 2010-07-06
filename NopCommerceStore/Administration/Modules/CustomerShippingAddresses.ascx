<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerShippingAddressesControl"
    CodeBehind="CustomerShippingAddresses.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="AddressDisplay" Src="AddressDisplay.ascx" %>
<asp:Button runat="server" ID="AddShippingAddress" CssClass="adminButton" OnClick="AddShippingAddress_Click"
    Text="<% $NopResources:Admin.CustomerShippingAddresses.AddButton.Text %>" ToolTip="<% $NopResources:Admin.CustomerShippingAddresses.AddButton.Tooltip %>" />
<asp:DataList ID="dlShippingAddresses" runat="server" RepeatColumns="3" RepeatDirection="Horizontal"
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
                        <%#GetLocaleResourceString("Admin.CustomerShippingAddresses.Edit")%>
                    </a>
                </td>
            </tr>
        </table>
    </ItemTemplate>
</asp:DataList>