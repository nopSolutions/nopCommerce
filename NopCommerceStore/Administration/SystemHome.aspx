<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="SystemHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.SystemHome" %>

<%@ Register Src="Modules/SystemHome.ascx" TagName="SystemHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:SystemHome ID="ctrlCatalogHome" runat="server" />
</asp:Content>
