<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/OneColumn.master" CodeBehind="Sitemap.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Sitemap" %>

<asp:Content runat="server" ContentPlaceHolderID="cph1">
    <ul class="sitemap">
        <asp:Literal runat="server" ID="lSitemapContent" EnableViewState="false" />
    </ul>
</asp:Content>
