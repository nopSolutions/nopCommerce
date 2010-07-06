<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="ConfigurationHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.ConfigurationHome" %>

<%@ Register Src="Modules/ConfigurationHome.ascx" TagName="ConfigurationHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:ConfigurationHome ID="ctrlConfigurationHome" runat="server" />
</asp:Content>
