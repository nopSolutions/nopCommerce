<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Title="Edit Post" Inherits="NopSolutions.NopCommerce.Web.Boards.MoveTopicPage"
    CodeBehind="MoveTopic.aspx.cs" ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="MoveForumTopic" Src="~/Modules/MoveForumTopic.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:MoveForumTopic ID="ctrlMoveForumTopic" runat="server" EditPost="true" />
</asp:Content>
