<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CreditCardTypeAdd"
    CodeBehind="CreditCardTypeAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CreditCardTypeAdd" Src="Modules/CreditCardTypeAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CreditCardTypeAdd runat="server" ID="ctrlCreditCardTypeAdd" />
</asp:Content>
