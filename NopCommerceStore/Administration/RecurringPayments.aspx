<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_RecurringPayments"
    CodeBehind="RecurringPayments.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="RecurringPayments" Src="Modules/RecurringPayments.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:RecurringPayments runat="server" ID="ctrlRecurringPayments" />
</asp:Content>
