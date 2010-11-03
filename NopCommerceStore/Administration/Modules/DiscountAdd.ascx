<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.DiscountAddControl"
    CodeBehind="DiscountAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DiscountInfo" Src="DiscountInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-promotions.png" alt="<%=GetLocaleResourceString("Admin.DiscountAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.DiscountAdd.Title")%>
        <a href="Discounts.aspx" title="<%=GetLocaleResourceString("Admin.DiscountAdd.BackToDiscounts")%>">
            (<%=GetLocaleResourceString("Admin.DiscountAdd.BackToDiscounts")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.DiscountAdd.AddButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.DiscountAdd.AddButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.DiscountAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:DiscountInfo ID="ctrlDiscountInfo" runat="server" />
