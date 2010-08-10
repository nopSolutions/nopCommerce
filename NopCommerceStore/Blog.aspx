<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.BlogPage" Codebehind="Blog.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Blog" Src="~/Modules/Blog.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="BlogMonths" Src="~/Modules/BlogMonths.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="BlogPopularTags" Src="~/Modules/BlogPopularTags.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph2" runat="server">
    <nopCommerce:BlogMonths ID="ctrlBlogMonths" runat="server" />
    <div class="clear">
    </div>
    <nopCommerce:BlogPopularTags ID="ctrlBlogPopularTags" runat="server" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:Blog ID="ctrlBlog" runat="server" />
</asp:Content>
