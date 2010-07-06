<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.StateProvinceDetailsControl"
    CodeBehind="StateProvinceDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="StateProvinceInfo" Src="StateProvinceInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.StateProvinceDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.StateProvinceDetails.Title")%>
        details <a href="StateProvinces.aspx" title="<%=GetLocaleResourceString("Admin.StateProvinceDetails.BackToStates")%>">
            (<%=GetLocaleResourceString("Admin.StateProvinceDetails.BackToStates")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.StateProvinceDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.StateProvinceDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.StateProvinceDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.StateProvinceDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:StateProvinceInfo ID="ctrlStateProvinceInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />