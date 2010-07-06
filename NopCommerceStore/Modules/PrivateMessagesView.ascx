<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.PrivateMessagesViewControl"
    CodeBehind="PrivateMessagesView.ascx.cs" %>
<div class="privatemessageview">
    <div class="title">
        <%=GetLocaleResourceString("PrivateMessages.View.ViewMessage")%>
    </div>
    <div class="wrapper">
        <table class="viewmessage">
            <tr>
                <td class="fieldname">
                    <%=GetLocaleResourceString("PrivateMessages.View.From")%>
                </td>
                <td>
                    <asp:Label ID="lblFrom" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="fieldname">
                    <%=GetLocaleResourceString("PrivateMessages.View.To")%>
                </td>
                <td>
                    <asp:Label ID="lblTo" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="fieldname">
                    <%=GetLocaleResourceString("PrivateMessages.View.Subject")%>
                </td>
                <td>
                    <asp:Label ID="lblSubject" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="fieldname">
                    <%=GetLocaleResourceString("PrivateMessages.View.Message")%>
                </td>
                <td class="message">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" class="options">
                    <asp:Button runat="server" ID="btnReply" OnClick="btnReply_Click" Text="<% $NopResources:PrivateMessages.View.ReplyButton %>"
                        CssClass="replypmbutton" />
                    <asp:Button runat="server" ID="btnDelete" OnClick="btnDelete_Click" Text="<% $NopResources:PrivateMessages.View.DeleteButton %>"
                        CssClass="deletepmbutton" />
                    <asp:Button runat="server" ID="btnBack" OnClick="btnBack_Click" Text="<% $NopResources:PrivateMessages.View.BackButton %>"
                        CssClass="backpmbutton" />
                </td>
            </tr>
        </table>
    </div>
</div>
