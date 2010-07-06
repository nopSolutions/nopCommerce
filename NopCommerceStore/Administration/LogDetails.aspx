<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_LogDetails"
    CodeBehind="LogDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="LogDetails" Src="Modules/LogDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:LogDetails runat="server" ID="ctrlLogDetails" />
</asp:Content>
