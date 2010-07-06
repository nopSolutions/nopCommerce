<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.PricelistAddControl"
    CodeBehind="PricelistAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="PricelistInfo" Src="PricelistInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-promotions.png" alt="<%=GetLocaleResourceString("Admin.PricelistAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.PricelistAdd.Title")%>
        <a href="PriceList.aspx" title="<%=GetLocaleResourceString("Admin.PricelistAdd.BackToPricelists")%>">
            (<%=GetLocaleResourceString("Admin.PricelistAdd.BackToPricelists")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.PricelistAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.PricelistAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:PricelistInfo ID="ctrlPricelistInfo" runat="server" />
