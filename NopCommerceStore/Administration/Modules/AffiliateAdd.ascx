<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.AffiliateAddControl"
    CodeBehind="AffiliateAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="AffiliateInfo" Src="AffiliateInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-promotions.png" alt="<%=GetLocaleResourceString("Admin.AffiliateAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.AffiliateAdd.Title")%>
        <a href="Affiliates.aspx" title="<%=GetLocaleResourceString("Admin.AffiliateAdd.BackToAffiliates")%>">
            (<%=GetLocaleResourceString("Admin.AffiliateAdd.BackToAffiliates")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.AffiliateAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.AffiliateAdd.SaveButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.AffiliateAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:AffiliateInfo ID="ctrlAffiliateInfo" runat="server" />
