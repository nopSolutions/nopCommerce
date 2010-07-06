<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_BlogPostDetails"
    CodeBehind="BlogPostDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="BlogPostDetails" Src="Modules/BlogPostDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:BlogPostDetails runat="server" ID="ctrlBlogPostDetails" />
</asp:Content>
