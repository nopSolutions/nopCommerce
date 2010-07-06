<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_PurchasedGiftCards"
    CodeBehind="PurchasedGiftCards.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="PurchasedGiftCards" Src="Modules/PurchasedGiftCards.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:PurchasedGiftCards runat="server" ID="ctrlPurchasedGiftCards" />
</asp:Content>