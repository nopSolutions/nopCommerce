<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.MoveForumTopicControl"
    CodeBehind="MoveForumTopic.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumBreadcrumb" Src="~/Modules/ForumBreadcrumb.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumSelector" Src="~/Modules/ForumSelector.ascx" %>
<div class="moveforumtopic">
    <nopCommerce:ForumBreadcrumb ID="ctrlForumBreadcrumb" runat="server" />
    <div class="title">
        <asp:Label ID="lblTitle" runat="server" Text="<% $NopResources:Forum.MoveTopic %>" />
    </div>
    <div class="wrapper">
        <asp:Panel runat="server" ID="pnlError" CssClass="error-block">
            <div class="message-error">
                <asp:Literal ID="lErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
            </div>
        </asp:Panel>
        <div class="clear">
        </div>
        <table class="movetopic">
            <tr>
                <td class="fieldname">
                    <%=GetLocaleResourceString("Forum.SelectTheForumToMoveTopic")%>:
                </td>
                <td>
                    <nopCommerce:ForumSelector ID="ctrlForumSelector" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2" class="options">
                    <asp:Button runat="server" ID="btnSubmit" OnClick="btnSubmit_Click" Text="<% $NopResources:Forum.Submit %>"
                        CssClass="submitforumtopicbutton" />
                    <asp:Button runat="server" ID="btnCancel" OnClick="btnCancel_Click" Text="<% $NopResources:Forum.Cancel %>"
                        CssClass="cancelforumtopicbutton" />
                </td>
            </tr>
        </table>
    </div>
</div>
