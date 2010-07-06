<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.WarehouseDetailsControl"
    CodeBehind="WarehouseDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="WarehouseInfo" Src="WarehouseInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.WarehouseDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.WarehouseDetails.Title")%>
        <a href="Warehouses.aspx" title="<%=GetLocaleResourceString("Admin.WarehouseDetails.BackToWarehouses")%>">
            (<%=GetLocaleResourceString("Admin.WarehouseDetails.BackToWarehouses")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.WarehouseDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.WarehouseDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.WarehouseDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.WarehouseDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:WarehouseInfo ID="ctrlWarehouseInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
