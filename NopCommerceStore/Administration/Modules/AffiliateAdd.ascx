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
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.AffiliateAdd.AddButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.AffiliateAdd.AddButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:AffiliateInfo ID="ctrlAffiliateInfo" runat="server" />
