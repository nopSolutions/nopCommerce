<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CampaignDetailsControl"
    CodeBehind="CampaignDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CampaignInfo" Src="CampaignInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-promotions.png" alt="<%=GetLocaleResourceString("Admin.CampaignDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.CampaignDetails.Title")%>
        <a href="Campaigns.aspx" title="<%=GetLocaleResourceString("Admin.CampaignDetails.BackToCampaign")%>">
            (<%=GetLocaleResourceString("Admin.CampaignDetails.BackToCampaign")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CampaignDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.CampaignDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CampaignDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.CampaignDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:CampaignInfo ID="ctrlCampaignInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
