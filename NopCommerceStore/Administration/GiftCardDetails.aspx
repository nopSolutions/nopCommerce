<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_GiftCardDetails"
    CodeBehind="GiftCardDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="GiftCardDetails" Src="Modules/GiftCardDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:GiftCardDetails runat="server" ID="ctrlGiftCardDetails" />
</asp:Content>
