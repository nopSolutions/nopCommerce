<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="PaymentSettingsHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.PaymentSettingsHome" %>

<%@ Register Src="Modules/PaymentHome.ascx" TagName="PaymentHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:PaymentHome ID="ctrlPaymentHome" runat="server" />
</asp:Content>
