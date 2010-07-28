<%@ Page Language="C#" MasterPageFile="~/Administration/popup.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_OrderPartialRefund"
    CodeBehind="OrderPartialRefund.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="OrderPartialRefund" Src="Modules/OrderPartialRefund.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:OrderPartialRefund runat="server" ID="ctrlOrderPartialRefund" />
</asp:Content>
