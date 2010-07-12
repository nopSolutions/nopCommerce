<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GiftCardAttributes.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Modules.GiftCardAttributesControl" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="~/Modules/SimpleTextBox.ascx" %>
<div class="giftCard">
    <dl>
        <dt>
            <asp:Label runat="server" ID="lblRecipientName" Text="<% $NopResources:Products.GiftCard.RecipientName %>"
                AssociatedControlID="txtRecipientName"></asp:Label></dt>
        <dd>
            <asp:TextBox runat="server" ID="txtRecipientName"></asp:TextBox></dd>
        <asp:PlaceHolder runat="server" ID="phRecipientEmail">
            <dt>
                <asp:Label runat="server" ID="lblRecipientEmail" Text="<% $NopResources:Products.GiftCard.RecipientEmail %>"
                    AssociatedControlID="txtRecipientEmail"></asp:Label></dt>
            <dd>
                <asp:TextBox runat="server" ID="txtRecipientEmail"></asp:TextBox></dd>
        </asp:PlaceHolder>
        <dt>
            <asp:Label runat="server" ID="lblSenderName" Text="<% $NopResources:Products.GiftCard.SenderName %>"
                AssociatedControlID="txtSenderName"></asp:Label></dt>
        <dd>
            <asp:TextBox runat="server" ID="txtSenderName"></asp:TextBox></dd>
        <asp:PlaceHolder runat="server" ID="phSenderEmail">
            <dt>
                <asp:Label runat="server" ID="lblSenderEmail" Text="<% $NopResources:Products.GiftCard.SenderEmail %>"
                    AssociatedControlID="txtSenderEmail"></asp:Label></dt>
            <dd>
                <asp:TextBox runat="server" ID="txtSenderEmail"></asp:TextBox></dd>
        </asp:PlaceHolder>
        <dt>
            <asp:Label runat="server" ID="lblGiftCardMessage" Text="<% $NopResources:Products.GiftCard.Message %>"
                AssociatedControlID="txtGiftCardMessage"></asp:Label></dt>
        <dd>
            <asp:TextBox runat="server" ID="txtGiftCardMessage" TextMode="MultiLine" Height="100px"
                Width="300px"></asp:TextBox></dd>
    </dl>
</div>
