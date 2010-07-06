<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Boards.ForumGroupPage" CodeBehind="ForumGroup.aspx.cs"
    ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ForumGroup" Src="~/Modules/ForumGroup.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumSearchBox" Src="~/Modules/ForumSearchBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumBreadcrumb" Src="~/Modules/ForumBreadcrumb.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="forumgrouppage">
        <nopCommerce:ForumBreadcrumb ID="ctrlForumBreadcrumb" runat="server" />
        <div class="clear">
        </div>
        <nopCommerce:ForumSearchBox ID="ctrlForumSearchBox" runat="server" />
        <div class="forumgroup">
            <nopCommerce:ForumGroup ID="ctrlForumGroup" runat="server" />
        </div>
        <div class="clear">
        </div>
    </div>
</asp:Content>
