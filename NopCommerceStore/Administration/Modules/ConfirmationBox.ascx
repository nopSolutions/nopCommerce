<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ConfirmationBox"
    CodeBehind="ConfirmationBox.ascx.cs" %>
<ajaxToolkit:ConfirmButtonExtender ID="cbe" runat="server" DisplayModalPopupID="mpe" />
<ajaxToolkit:ModalPopupExtender ID="mpe" runat="server" PopupControlID="pnlPopup"
    OkControlID="btnYES" CancelControlID="btnNO" BackgroundCssClass="modalBackground" />
<asp:Panel ID="pnlPopup" runat="server" Style="display: none; width: 250px; background-color: White;
    border-width: 2px; border-color: Black; border-style: solid; padding: 20px;">
    <div style="text-align: center;">
        <asp:Literal runat="server" ID="lConfirmText"></asp:Literal>
        <p>
        </p>
        <asp:Button ID="btnYES" runat="server" Text="Yes" CssClass="adminButton" CausesValidation="false" />
        <asp:Button ID="btnNO" runat="server" Text="No" CssClass="adminButton" CausesValidation="false" />
    </div>
</asp:Panel>
