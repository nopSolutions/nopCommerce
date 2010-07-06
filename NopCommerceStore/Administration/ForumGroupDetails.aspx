<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ForumGroupDetails"
    CodeBehind="ForumGroupDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ForumGroupDetails" Src="Modules/ForumGroupDetails.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:ForumGroupDetails runat="server" ID="ctrlForumGroupDetails" />
</asp:Content>
