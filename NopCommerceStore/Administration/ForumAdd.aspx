<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ForumAdd"
    CodeBehind="ForumAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ForumAdd" Src="Modules/ForumAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ForumAdd runat="server" ID="ctrlForumAdd" />
</asp:Content>