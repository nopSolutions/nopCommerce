<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.NewsCommentDetailsControl"
    CodeBehind="NewsCommentDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.NewsCommentDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.NewsCommentDetails.Title")%>
        <a href="NewsComments.aspx" title="<%=GetLocaleResourceString("Admin.NewsCommentDetails.BackToNewsComments")%>">
            (<%=GetLocaleResourceString("Admin.NewsCommentDetails.BackToNewsComments")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.NewsCommentDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.NewsCommentDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.NewsCommentDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.NewsCommentDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerTitle" Text="<% $NopResources:Admin.NewsCommentDetails.Customer %>"
                ToolTip="<% $NopResources:Admin.NewsCommentDetails.Customer.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblCustomer" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblIPAddressTitle" Text="<% $NopResources:Admin.NewsCommentDetails.IPAddress %>"
                ToolTip="<% $NopResources:Admin.NewsCommentDetails.IPAddress.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblIPAddress" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblNewsTitle" Text="<% $NopResources:Admin.NewsCommentDetails.News %>"
                ToolTip="<% $NopResources:Admin.NewsCommentDetails.News.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblNews" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTitle" Text="<% $NopResources:Admin.NewsCommentDetails.CommentTitle %>"
                ToolTip="<% $NopResources:Admin.NewsCommentDetails.CommentTitle.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" CssClass="adminInput" ID="txtTitle">
            </asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblComment" Text="<% $NopResources:Admin.NewsCommentDetails.CommentBody %>"
                ToolTip="<% $NopResources:Admin.NewsCommentDetails.CommentBody.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtComment" TextMode="MultiLine" Height="150px" Width="500px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreatedOnTitle" Text="<% $NopResources:Admin.NewsCommentDetails.CreatedOn %>"
                ToolTip="<% $NopResources:Admin.NewsCommentDetails.CreatedOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblCreatedOn" runat="server"></asp:Label>
        </td>
    </tr>
</table>
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
