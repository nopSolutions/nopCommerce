<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="ContentManagementHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.ContentManagementHome" %>

<%@ Register Src="Modules/ContentManagementHome.ascx" TagName="ContentManagementHome"
    TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:ContentManagementHome ID="ctrlContentManagementHome" runat="server" />
</asp:Content>
