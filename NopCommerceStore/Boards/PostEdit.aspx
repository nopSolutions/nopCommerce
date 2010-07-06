<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Title="Edit Post" Inherits="NopSolutions.NopCommerce.Web.Boards.PostEditPage" CodeBehind="PostEdit.aspx.cs"
    ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ForumPostEdit" Src="~/Modules/ForumPostEdit.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="postedit">
        <nopCommerce:ForumPostEdit ID="ctrlForumPostEdit" runat="server" EditPost="true" />
    </div>
</asp:Content>
