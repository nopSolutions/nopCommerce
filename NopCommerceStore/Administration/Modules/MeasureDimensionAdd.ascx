<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.MeasureDimensionAddControl"
    CodeBehind="MeasureDimensionAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="MeasureDimensionInfo" Src="MeasureDimensionInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.Measures.MeasureDimensionAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.Measures.MeasureDimensionAdd.Title")%>
        <a href="Measures.aspx" title="<%=GetLocaleResourceString("Admin.Measures.MeasureDimensionAdd.BackToMeasures")%>">
            (<%=GetLocaleResourceString("Admin.Measures.MeasureDimensionAdd.BackToMeasures")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.Measures.MeasureDimensionAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.Measures.MeasureDimensionAdd.SaveButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.MeasureDimensionAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:MeasureDimensionInfo ID="ctrlMeasureDimensionInfo" runat="server" />
