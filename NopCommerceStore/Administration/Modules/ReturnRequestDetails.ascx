<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ReturnRequestDetailsControl"
    CodeBehind="ReturnRequestDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ReturnRequestInfo" Src="ReturnRequestInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-sales.png" alt="<%=GetLocaleResourceString("Admin.ReturnRequestDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.ReturnRequestDetails.Title")%>
        <a href="ReturnRequests.aspx" title="<%=GetLocaleResourceString("Admin.ReturnRequestDetails.BackToReturnRequests")%>">
            (<%=GetLocaleResourceString("Admin.ReturnRequestDetails.BackToReturnRequests")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ReturnRequestDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ReturnRequestDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ReturnRequestDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.ReturnRequestDetails.DeleteButton.Tooltip %>" />
        <nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
            YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
            ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
    </div>
</div>
<nopCommerce:ReturnRequestInfo ID="ctrlReturnRequestInfo" runat="server" />
