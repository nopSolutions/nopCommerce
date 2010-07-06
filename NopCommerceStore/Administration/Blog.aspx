<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Blog" CodeBehind="Blog.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Blog" Src="Modules/Blog.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Blog runat="server" ID="ctrlBlog" />
</asp:Content>
