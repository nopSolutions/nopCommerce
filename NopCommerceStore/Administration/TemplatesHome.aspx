<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="TemplatesHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.TemplatesHome" %>

<%@ Register Src="Modules/TemplatesHome.ascx" TagName="TemplatesHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:TemplatesHome ID="ctrlTemplatesHome" runat="server" />
</asp:Content>
