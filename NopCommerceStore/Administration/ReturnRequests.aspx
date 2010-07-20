<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ReturnRequestsReport"
    CodeBehind="ReturnRequests.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ReturnRequests" Src="Modules/ReturnRequests.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ReturnRequests runat="server" ID="ctrlReturnRequests" />
</asp:Content>
