<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ForumBreadcrumbControl"
    CodeBehind="ForumBreadcrumb.ascx.cs" %>
<div class="forumbreadcrumb">
    <asp:HyperLink runat="server" ID="hlHome" Text="<% $NopResources:Forum.Home %>" />
    /
    <asp:HyperLink runat="server" ID="hlForumsHome" Text="<% $NopResources:Forum.Forums %>" />
    <asp:PlaceHolder runat="server" ID="phForumGroup">/
        <asp:HyperLink runat="server" ID="hlForumGroup" /></asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phForum">/
        <asp:HyperLink runat="server" ID="hlForum" /></asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="phForumTopic">/
        <asp:HyperLink runat="server" ID="hlForumTopic" /></asp:PlaceHolder>
</div>
