<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BlogPostDetailsControl"
    CodeBehind="BlogPostDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="BlogPostInfo" Src="BlogPostInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.BlogPostDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.BlogPostDetails.Title")%>
        <a href="Blog.aspx" title="<%=GetLocaleResourceString("Admin.BlogPostDetails.BackToBlog")%>">
            (<%=GetLocaleResourceString("Admin.BlogPostDetails.BackToBlog")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.BlogPostDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.BlogPostDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.BlogPostDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.BlogPostDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:BlogPostInfo ID="ctrlBlogPostInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
