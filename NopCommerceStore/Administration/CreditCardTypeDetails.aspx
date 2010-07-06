<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CreditCardTypeDetails"
    CodeBehind="CreditCardTypeDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CreditCardTypeDetails" Src="Modules/CreditCardTypeDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CreditCardTypeDetails runat="server" ID="ctrlCreditCardTypeDetails" />
</asp:Content>
