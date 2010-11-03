<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CampaignAddControl"
    CodeBehind="CampaignAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CampaignInfo" Src="CampaignInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-promotions.png" alt="<%=GetLocaleResourceString("Admin.Campaigns.CampaignAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.Campaigns.CampaignAdd.Title")%>
        <a href="Campaigns.aspx" title="<%=GetLocaleResourceString("Admin.Campaigns.CampaignAdd.BackToCampaign")%>">
            (<%=GetLocaleResourceString("Admin.Campaigns.CampaignAdd.BackToCampaign")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.Campaigns.AddButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.Campaigns.AddButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CampaignAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:CampaignInfo ID="ctrlCampaignInfo" runat="server" />
