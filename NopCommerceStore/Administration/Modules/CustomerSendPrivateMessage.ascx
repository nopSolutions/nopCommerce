<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerSendPrivateMessage"
    CodeBehind="CustomerSendPrivateMessage.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>

<asp:Panel runat="server" ID="pnlNotAllowed">
    <%=GetLocaleResourceString("Admin.CustomerSendPrivateMessage.NotAllowed")%>
</asp:Panel>
<asp:Panel runat="server" ID="pnlSendPriveteMessage">
    <table class="adminContent">
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblSubject" Text="<% $NopResources:Admin.CustomerSendPrivateMessage.Subject %>"
                    ToolTip="<% $NopResources:Admin.CustomerSendPrivateMessage.Subject.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:TextBox runat="server" ID="txtSubject"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvSubject" runat="server" ControlToValidate="txtSubject"
                    ErrorMessage="<% $NopResources:Admin.CustomerSendPrivateMessage.Subject.Required %>" ValidationGroup="SendPM">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblBody" Text="<% $NopResources:Admin.CustomerSendPrivateMessage.Body %>"
                    ToolTip="<% $NopResources:Admin.CustomerSendPrivateMessage.Body.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">

                <script language="javascript" type="text/javascript">
                    var webRoot = '<%=CommonHelper.GetStoreLocation()%>';
                    edToolbar('<%=txtMessageBBCode.ClientID %>'); 
                </script>

                <asp:TextBox ID="txtMessageBBCode" runat="server" Width="100%" Height="350px" TextMode="MultiLine"/>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="adminData">
                <asp:Button runat="server" ID="btnSend" CssClass="adminButton" OnClick="btnSend_Click"
                    Text="<% $NopResources:Admin.CustomerSendPrivateMessage.SendButton %>"
                    ValidationGroup="SendPM" />
            </td>
        </tr>
    </table>
</asp:Panel>
