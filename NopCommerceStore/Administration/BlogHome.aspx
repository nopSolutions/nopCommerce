<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="BlogHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.BlogHome" %>

<%@ Register TagPrefix="nopCommerce" TagName="BlogHome" Src="Modules/BlogHome.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:BlogHome runat="server" ID="ctrlBlogHome" />
</asp:Content>
