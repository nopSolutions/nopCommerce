<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_RecurringPaymentDetails"
    CodeBehind="RecurringPaymentDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="RecurringPaymentDetails" Src="Modules/RecurringPaymentDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:RecurringPaymentDetails runat="server" ID="ctrlRecurringPaymentDetails" />
</asp:Content>
