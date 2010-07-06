<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Title="New Post" Inherits="NopSolutions.NopCommerce.Web.Boards.PostNewPage" CodeBehind="PostNew.aspx.cs"
    ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ForumPostEdit" Src="~/Modules/ForumPostEdit.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:ForumPostEdit ID="ctrlForumPostEdit" runat="server" AddPost="true" />
</asp:Content>
