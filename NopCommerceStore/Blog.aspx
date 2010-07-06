<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.BlogPage" Codebehind="Blog.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Blog" Src="~/Modules/Blog.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:Blog ID="ctrlBlog" runat="server" />
</asp:Content>
