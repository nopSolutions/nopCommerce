<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.MeasureWeightAddControl"
    CodeBehind="MeasureWeightAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="MeasureWeightInfo" Src="MeasureWeightInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.Measures.MeasureWeightAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.Measures.MeasureWeightAdd.Title")%>
        <a href="Measures.aspx" title="<%=GetLocaleResourceString("Admin.Measures.MeasureWeightAdd.BackToMeasures")%>">
            (<%=GetLocaleResourceString("Admin.Measures.MeasureWeightAdd.BackToMeasures")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.Measures.MeasureWeightAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.Measures.MeasureWeightAdd.SaveButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.MeasureWeightAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:MeasureWeightInfo ID="ctrlMeasureWeightInfo" runat="server" />
