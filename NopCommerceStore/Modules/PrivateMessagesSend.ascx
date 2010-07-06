<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.PrivateMessagesSendControl"
    CodeBehind="PrivateMessagesSend.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="~/Modules/SimpleTextBox.ascx" %>
<div class="privatemessagesend">
    <div class="title">
        <%=GetLocaleResourceString("PrivateMessages.Send.PostMessage")%>
    </div>
    <div class="wrapper">
        <asp:Panel runat="server" ID="pnlError" Visible="false" CssClass="error-block">
            <div class="message-error">
                <asp:Literal ID="lErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
            </div>
        </asp:Panel>
        <div class="clear">
        </div>
        <table class="postmessage">
            <tr>
                <td class="fieldname">
                    <%=GetLocaleResourceString("PrivateMessages.Send.To")%>
                </td>
                <td>
                    <asp:Label ID="lblSendTo" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="fieldname">
                    <%=GetLocaleResourceString("PrivateMessages.Send.Subject")%>
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtSubject" ValidationGroup="Submit" SkinID="PMSubjectText"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSubject" runat="server" ControlToValidate="txtSubject"
                        ErrorMessage="<% $NopResources:PrivateMessages.Send.Subject.Required %>" ValidationGroup="Submit">*</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="fieldname">
                    <%=GetLocaleResourceString("PrivateMessages.Send.Message")%>
                </td>
                <td>

                    <script language="javascript" type="text/javascript">
                        var webRoot = '<%=CommonHelper.GetStoreLocation()%>';
                        edToolbar('<%=txtMessageBBCode.ClientID %>'); 
                    </script>

                    <asp:TextBox ID="txtMessageBBCode" runat="server" Width="100%" Height="300px" TextMode="MultiLine"
                        SkinID="BBEditorText" />
                </td>
            </tr>
            <tr>
                <td colspan="2" class="options">
                    <asp:Button runat="server" ID="btnSend" OnClick="btnSend_Click" Text="<% $NopResources:PrivateMessages.Send.SendButton %>"
                        CssClass="submitpmbutton" ValidationGroup="Submit" />
                    <asp:Button runat="server" ID="btnCancel" OnClick="btnCancel_Click" Text="<% $NopResources:PrivateMessages.Send.CancelButton %>"
                        CssClass="cancelpmbutton" />
                </td>
            </tr>
        </table>
    </div>
</div>
