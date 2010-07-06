<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.OrderProgressControl"
    CodeBehind="OrderProgress.ascx.cs" %>
<div class="order-progress">
    <ul>
        <li>
            <asp:HyperLink runat="server" ID="hlCart" Text="<% $NopResources:OrderProgress.Cart %>">
            </asp:HyperLink></li>
        <li>
            <asp:HyperLink runat="server" ID="hlAddress" Text="<% $NopResources:OrderProgress.Address %>">
            </asp:HyperLink></li>
        <li>
            <asp:HyperLink runat="server" ID="hlShipping" Text="<% $NopResources:OrderProgress.Shipping %>">
            </asp:HyperLink></li>
        <li>
            <asp:HyperLink runat="server" ID="hlPayment" Text="<% $NopResources:OrderProgress.Payment %>">
            </asp:HyperLink></li>
        <li>
            <asp:HyperLink runat="server" ID="hlConfirm" Text="<% $NopResources:OrderProgress.Confirm %>">
            </asp:HyperLink></li>
        <li>
            <asp:HyperLink runat="server" ID="hlComplete" Text="<% $NopResources:OrderProgress.Complete %>">
            </asp:HyperLink></li>
    </ul>
</div>
