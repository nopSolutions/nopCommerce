<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Boards.DefaultPage" CodeBehind="Default.aspx.cs"
    ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ForumGroups" Src="~/Modules/ForumGroups.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumSearchBox" Src="~/Modules/ForumSearchBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="Topic" Src="~/Modules/Topic.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumActiveDiscussions" Src="~/Modules/ForumActiveDiscussions.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="forumsmain">
        <div>
            <nopCommerce:Topic ID="topicForumWelcomeMessage" runat="server" TopicName="ForumWelcomeMessage"
                OverrideSEO="false"></nopCommerce:Topic>
        </div>
        <div class="clear">
        </div>
        <div class="forumsmainheader">
            <div class="currenttime">
                <asp:Label runat="server" ID="lblCurrentTime" />
            </div>
            <nopCommerce:ForumSearchBox runat="server" ID="ctrlForumSearchBox" />
        </div>
        <div class="clear">
        </div>
        <nopCommerce:ForumGroups ID="ctrlForumGroups" runat="server" />
        <div class="clear">
        </div>
        <nopCommerce:ForumActiveDiscussions runat="server" ID="ctrlForumActiveDiscussions"
            ForumID="0" />
    </div>
</asp:Content>
