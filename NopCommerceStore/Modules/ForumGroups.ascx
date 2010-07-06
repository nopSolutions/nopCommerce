<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ForumGroupsControl"
    CodeBehind="ForumGroups.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumGroup" Src="~/Modules/ForumGroup.ascx" %>
<div class="forumgroups">
    <asp:Repeater ID="rptrForumGroups" runat="server">
        <ItemTemplate>
            <nopCommerce:ForumGroup ID="ctrlForumGroup" runat="server" ForumGroupID='<%#Eval("ForumGroupId")%>' />
        </ItemTemplate>
    </asp:Repeater>
</div>
