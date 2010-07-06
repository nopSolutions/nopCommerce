<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Boards.ForumPage" CodeBehind="Forum.aspx.cs"
    ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="Forum" Src="~/Modules/Forum.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="forum">
        <nopCommerce:Forum ID="ctrlForum" runat="server" />
    </div>
</asp:Content>
