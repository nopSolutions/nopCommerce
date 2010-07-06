<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ForumDetails"
    CodeBehind="ForumDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ForumDetails" Src="Modules/ForumDetails.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:ForumDetails runat="server" ID="ctrlForumDetails" />
</asp:Content>
