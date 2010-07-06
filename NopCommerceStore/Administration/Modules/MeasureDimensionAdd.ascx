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
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.Measures.MeasureDimensionAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.Measures.MeasureDimensionAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:MeasureDimensionInfo ID="ctrlMeasureDimensionInfo" runat="server" />
