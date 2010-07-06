<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductAttributeDetailsControl"
    CodeBehind="ProductAttributeDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductAttributeInfo" Src="ProductAttributeInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.ProductAttributeDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.ProductAttributeDetails.Title")%><a href="ProductAttributes.aspx"
            title="<%=GetLocaleResourceString("Admin.ProductAttributeDetails.BackToAttributeList")%>">
            (<%=GetLocaleResourceString("Admin.ProductAttributeDetails.BackToAttributeList")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ProductAttributeDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ProductAttributeDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ProductAttributeDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.ProductAttributeDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:ProductAttributeInfo ID="ctrlProductAttributeInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />