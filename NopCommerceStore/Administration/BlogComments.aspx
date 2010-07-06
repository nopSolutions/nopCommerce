<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_BlogComments"
    CodeBehind="BlogComments.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="BlogComments" Src="Modules/BlogComments.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:BlogComments runat="server" ID="ctrlBlogComments" />
</asp:Content>
