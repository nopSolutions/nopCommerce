<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Boards.TopicPage" CodeBehind="Topic.aspx.cs"
    ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ForumTopic" Src="~/Modules/ForumTopic.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="topic">
        <nopCommerce:ForumTopic ID="ctrlForumTopic" runat="server" />
    </div>
</asp:Content>
