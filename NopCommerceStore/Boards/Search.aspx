<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Boards.SearchPage" ValidateRequest="false"
    CodeBehind="Search.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ForumSearch" Src="~/Modules/ForumSearch.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="forumsearch">
        <nopCommerce:ForumSearch ID="ctrlForumSearch" runat="server" />
    </div>
</asp:Content>
