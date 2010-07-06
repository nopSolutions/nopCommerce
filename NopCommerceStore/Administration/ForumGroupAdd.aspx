<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ForumGroupAdd"
    CodeBehind="ForumGroupAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ForumGroupAdd" Src="Modules/ForumGroupAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ForumGroupAdd runat="server" ID="ctrlForumGroupAdd" />
</asp:Content>