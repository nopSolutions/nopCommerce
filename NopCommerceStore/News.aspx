<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.NewsPage" CodeBehind="News.aspx.cs"  %>

<%@ Register TagPrefix="nopCommerce" TagName="NewsItem" Src="~/Modules/NewsItem.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:NewsItem ID="ctrlNewsItem" runat="server" />
</asp:Content>
