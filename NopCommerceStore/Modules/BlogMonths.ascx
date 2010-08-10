<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.BlogMonthsControl"
    CodeBehind="BlogMonths.ascx.cs" %>
<div class="block block-blog-archive">
    <div class="title">
        <%=GetLocaleResourceString("Blog.Archive")%>
    </div>
    <div class="clear">
    </div>
    <div class="listbox">
        <asp:Literal runat="server" ID="lMonths" EnableViewState="false" />
    </div>
</div>
    
    
    
    