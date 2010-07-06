<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_PaymentMethods"
    CodeBehind="PaymentMethods.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="PaymentMethods" Src="Modules/PaymentMethods.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:PaymentMethods runat="server" ID="ctrlPaymentMethods" />
</asp:Content>
