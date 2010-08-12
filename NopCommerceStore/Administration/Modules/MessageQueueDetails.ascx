<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.MessageQueueDetailsControl"
    CodeBehind="MessageQueueDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<%@ Register Assembly="NopCommerceStore" Namespace="NopSolutions.NopCommerce.Web.Controls"
    TagPrefix="nopCommerce" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-promotions.png" alt="<%=GetLocaleResourceString("Admin.MessageQueueDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.MessageQueueDetails.Title")%>
        <a href="MessageQueue.aspx" title="<%=GetLocaleResourceString("Admin.MessageQueueDetails.BackToEmails")%>">
            (<%=GetLocaleResourceString("Admin.MessageQueueDetails.BackToEmails")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="RequeueButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.MessageQueueDetails.RequeueButton.Text %>"
            OnClick="RequeueButton_Click" ToolTip="<% $NopResources:Admin.MessageQueueDetails.RequeueButton.Tooltip %>" />
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.MessageQueueDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.MessageQueueDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.MessageQueueDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.MessageQueueDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPriority" Text="<% $NopResources:Admin.MessageQueueDetails.Priority %>"
                ToolTip="<% $NopResources:Admin.MessageQueueDetails.Priority.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtPriority"
                RequiredErrorMessage="<% $NopResources:Admin.MessageQueueDetails.Priority.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" Value="0" RangeErrorMessage="<% $NopResources:Admin.MessageQueueDetails.Priority.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblFrom" Text="<% $NopResources:Admin.MessageQueueDetails.From %>"
                ToolTip="<% $NopResources:Admin.MessageQueueDetails.From.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtFrom" ErrorMessage="<% $NopResources:Admin.MessageQueueDetails.From.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblFromName" Text="<% $NopResources:Admin.MessageQueueDetails.FromName %>"
                ToolTip="<% $NopResources:Admin.MessageQueueDetails.FromName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtFromName" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTo" Text="<% $NopResources:Admin.MessageQueueDetails.To %>"
                ToolTip="<% $NopResources:Admin.MessageQueueDetails.To.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtTo" ErrorMessage="<% $NopResources:Admin.MessageQueueDetails.To.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblToName" Text="<% $NopResources:Admin.MessageQueueDetails.ToName %>"
                ToolTip="<% $NopResources:Admin.MessageQueueDetails.ToName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtToName" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCC" Text="<% $NopResources:Admin.MessageQueueDetails.CC %>"
                ToolTip="<% $NopResources:Admin.MessageQueueDetails.CC.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtCc" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblBcc" Text="<% $NopResources:Admin.MessageQueueDetails.Bcc %>"
                ToolTip="<% $NopResources:Admin.MessageQueueDetails.Bcc.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtBcc" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblSubject" Text="<% $NopResources:Admin.MessageQueueDetails.Subject %>"
                ToolTip="<% $NopResources:Admin.MessageQueueDetails.Subject.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtSubject" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblBody" Text="<% $NopResources:Admin.MessageQueueDetails.Body %>"
                ToolTip="<% $NopResources:Admin.MessageQueueDetails.Body.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NopHTMLEditor ID="txtBody" runat="server" Height="350" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreatedOnTitle" Text="<% $NopResources:Admin.MessageQueueDetails.CreatedOn %>"
                ToolTip="<% $NopResources:Admin.MessageQueueDetails.CreatedOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblCreatedOn" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblSendTries" Text="<% $NopResources:Admin.MessageQueueDetails.SendTries %>"
                ToolTip="<% $NopResources:Admin.MessageQueueDetails.SendTries.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtSendTries"
                RequiredErrorMessage="<% $NopResources:Admin.MessageQueueDetails.SendTries.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" Value="0" RangeErrorMessage="<% $NopResources:Admin.MessageQueueDetails.SendTries.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblSendOnTitle" Text="<% $NopResources:Admin.MessageQueueDetails.SendOn %>"
                ToolTip="<% $NopResources:Admin.MessageQueueDetails.SendOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblSentOn" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblEmailAccountTitle" Text="<% $NopResources:Admin.MessageQueueDetails.EmailAccount %>"
                ToolTip="<% $NopResources:Admin.MessageQueueDetails.EmailAccount.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblEmailAccount" runat="server"></asp:Label>
        </td>
    </tr>
</table>
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
