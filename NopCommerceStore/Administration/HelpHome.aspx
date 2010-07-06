<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="HelpHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.HelpHome" %>

<%@ Register Src="Modules/HelpHome.ascx" TagName="HelpHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:HelpHome ID="ctrlHelpHome" runat="server" />
</asp:Content>
