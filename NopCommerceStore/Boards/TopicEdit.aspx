<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Title="Topic Edit" Inherits="NopSolutions.NopCommerce.Web.Boards.TopicEditPage"
    CodeBehind="TopicEdit.aspx.cs" ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ForumPostEdit" Src="~/Modules/ForumPostEdit.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="topicedit">
        <nopCommerce:ForumPostEdit ID="ctrlForumPostEdit" runat="server" EditTopic="true" />
    </div>
</asp:Content>
