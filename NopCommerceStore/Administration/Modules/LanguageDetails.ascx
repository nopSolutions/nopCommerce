<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.LanguageDetailsControl"
    CodeBehind="LanguageDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="LanguageInfo" Src="LanguageInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.LanguageDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.LanguageDetails.Title")%>
        <a href="Languages.aspx" title="<%=GetLocaleResourceString("Admin.LanguageDetails.BackToLanguages")%>">
            (<%=GetLocaleResourceString("Admin.LanguageDetails.BackToLanguages")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="btnXmlExport" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.LanguageDetails.BtnXmlExport.Text %>" OnClick="BtnXmlExport_OnClick" ToolTip="<% $NopResources:Admin.LanguageDetails.BtnXmlExport.Tooltip %>" />
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.LanguageDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.LanguageDetails.SaveButton.Tooltip %>">
        </asp:Button>
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.LanguageDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.LanguageDetails.DeleteButton.Tooltip %>">
        </asp:Button>
    </div>
</div>
<nopCommerce:LanguageInfo ID="ctrlLanguageInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
