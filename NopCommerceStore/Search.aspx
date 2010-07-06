<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.SearchPage" Codebehind="Search.aspx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="Search" Src="~/Modules/Search.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:Search ID="ctrlSearch" runat="server" />
</asp:Content>
