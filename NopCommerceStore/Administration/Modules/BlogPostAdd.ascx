<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BlogPostAddControl"
    CodeBehind="BlogPostAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="BlogPostInfo" Src="BlogPostInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.BlogPostAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.BlogPostAdd.Title")%>
        <a href="Blog.aspx" title="<%=GetLocaleResourceString("Admin.BlogPostAdd.BackToBlog")%>">
            (<%=GetLocaleResourceString("Admin.BlogPostAdd.BackToBlog")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.BlogPostAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.BlogPostAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:BlogPostInfo ID="ctrlBlogPostInfo" runat="server" />
