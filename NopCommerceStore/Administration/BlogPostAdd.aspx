<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_BlogPostAdd"
    CodeBehind="BlogPostAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="BlogPostAdd" Src="Modules/BlogPostAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:BlogPostAdd runat="server" ID="ctrlBlogPostAdd" />
</asp:Content>
