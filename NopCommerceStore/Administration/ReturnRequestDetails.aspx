<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ReturnRequestDetails"
    CodeBehind="ReturnRequestDetails.aspx.cs" ValidateRequest="false"  %>

<%@ Register TagPrefix="nopCommerce" TagName="ReturnRequestDetails" Src="Modules/ReturnRequestDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ReturnRequestDetails runat="server" ID="ctrlReturnRequestDetails" />
</asp:Content>
