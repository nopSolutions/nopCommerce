<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Title="New Topic" Inherits="NopSolutions.NopCommerce.Web.Boards.TopicNewPage"
    CodeBehind="TopicNew.aspx.cs" ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ForumPostEdit" Src="~/Modules/ForumPostEdit.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="topicedit">
        <nopCommerce:ForumPostEdit ID="ctrlForumPostEdit" runat="server" AddTopic="true"  />
    </div>
</asp:Content>
