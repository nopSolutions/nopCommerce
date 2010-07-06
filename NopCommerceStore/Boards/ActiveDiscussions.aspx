<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Boards.ActiveDiscussionsPage" CodeBehind="ActiveDiscussions.aspx.cs"
    ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ForumActiveDiscussions" Src="~/Modules/ForumActiveDiscussions.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="activediscussionspage">
        <nopCommerce:ForumActiveDiscussions runat="server" ID="ctrlForumActiveDiscussions"
            ForumID="0" TopicCount="50" HideViewAllLink="true" />
    </div>
</asp:Content>
