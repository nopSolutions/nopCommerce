<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.InfoBlockControl"
    CodeBehind="InfoBlock.ascx.cs" %>
<div class="block block-info">
    <div class="title">
        <%=GetLocaleResourceString("Content.Information")%>
    </div>
    <div class="clear">
    </div>
    <div class="listbox">
        <ul>
            <li><a href="<%=Page.ResolveUrl("~/contactus.aspx")%>">
                <%=GetLocaleResourceString("ContactUs.ContactUs")%></a> </li>
            <li><a href="<%=Page.ResolveUrl("~/aboutus.aspx")%>">
                <%=GetLocaleResourceString("Content.AboutUs")%></a></li>
            <% if (BlogManager.BlogEnabled)
               { %>
            <li><a href="<%= SEOHelper.GetBlogUrl()%>">
                <%=GetLocaleResourceString("Blog.Blog")%></a></li>
            <%} %>
            <% if (ForumManager.ForumsEnabled)
               { %>
            <li><a href="<%= SEOHelper.GetForumMainUrl()%> ">
                <%=GetLocaleResourceString("Forum.Forums")%></a></li>
            <%} %>
            <% if (ProductManager.RecentlyAddedProductsEnabled)
               { %>
            <li><a href="<%=Page.ResolveUrl("~/recentlyaddedproducts.aspx")%>">
                <%=GetLocaleResourceString("Products.NewProducts")%></a></li>
            <%} %>
            <% if (ProductManager.RecentlyViewedProductsEnabled)
               { %>
            <li><a href="<%=Page.ResolveUrl("~/recentlyviewedproducts.aspx")%>">
                <%=GetLocaleResourceString("Products.RecentlyViewedProducts")%></a></li>
            <%} %>
            <% if (ProductManager.CompareProductsEnabled)
               { %>
            <li><a href="<%=Page.ResolveUrl("~/compareproducts.aspx")%>">
                <%=GetLocaleResourceString("Products.CompareProductsList")%></a></li>
            <%} %>
            <li><a href="<%=Page.ResolveUrl("~/sitemap.aspx")%>">
                <%=GetLocaleResourceString("Content.Sitemap")%></a></li>
            <li><a href="<%=Page.ResolveUrl("~/shippinginfo.aspx")%>">
                <%=GetLocaleResourceString("Content.Shipping&Returns")%></a></li>
            <li><a href="<%=Page.ResolveUrl("~/privacyinfo.aspx")%>">
                <%=GetLocaleResourceString("Content.PrivacyNotice")%></a></li>
            <li><a href="<%=Page.ResolveUrl("~/conditionsinfo.aspx")%>">
                <%=GetLocaleResourceString("Content.ConditionsOfUse")%></a></li>
        </ul>
    </div>
</div>
