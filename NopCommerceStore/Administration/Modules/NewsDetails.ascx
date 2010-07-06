<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.NewsDetailsControl"
    CodeBehind="NewsDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NewsInfo" Src="NewsInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.NewsDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.NewsDetails.Title")%>
        <a href="News.aspx" title="<%=GetLocaleResourceString("Admin.NewsDetails.BackToNews")%>">
            (<%=GetLocaleResourceString("Admin.NewsDetails.BackToNews")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.NewsDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.NewsDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.NewsDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.NewsDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:NewsInfo ID="ctrlNewsInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />