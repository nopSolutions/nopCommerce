<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_BlogSettings"
    CodeBehind="BlogSettings.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="BlogSettings" Src="Modules/BlogSettings.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:BlogSettings runat="server" ID="ctrlBlogSettings" />
</asp:Content>
