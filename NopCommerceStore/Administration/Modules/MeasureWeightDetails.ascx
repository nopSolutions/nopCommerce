<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.MeasureWeightDetailsControl"
    CodeBehind="MeasureWeightDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="MeasureWeightInfo" Src="MeasureWeightInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.Measures.MeasureWeightDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.Measures.MeasureWeightDetails.Title")%>
        <a href="Measures.aspx" title="<%=GetLocaleResourceString("Admin.Measures.MeasureWeightDetails.BackToMeasures")%>">
            (<%=GetLocaleResourceString("Admin.Measures.MeasureWeightDetails.BackToMeasures")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Measures.MeasureWeightDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.Measures.MeasureWeightDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Measures.MeasureWeightDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.Measures.MeasureWeightDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:MeasureWeightInfo ID="ctrlMeasureWeightInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
