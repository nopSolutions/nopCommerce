<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CreditCardTypes"
    CodeBehind="CreditCardTypes.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CreditCardTypes" Src="Modules/CreditCardTypes.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CreditCardTypes runat="server" ID="ctrlCreditCardTypes" />
</asp:Content>
