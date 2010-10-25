<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.HeaderMenuControl"
    CodeBehind="HeaderMenu.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SearchBox" Src="~/Modules/SearchBox.ascx" %>
<div class="headermenu">
    <div class="searchbox">
        <nopCommerce:SearchBox runat="server" ID="ctrlSearchBox">
        </nopCommerce:SearchBox>
    </div>
    <ul class="topmenu">
        <li><a href="<%=CommonHelper.GetStoreLocation()%>">
            <%=GetLocaleResourceString("Content.HomePage")%></a> </li>
        <% if (IoCFactory.Resolve<IProductManager>().RecentlyAddedProductsEnabled)
           { %>
        <li><a href="<%=Page.ResolveUrl("~/recentlyaddedproducts.aspx")%>">
            <%=GetLocaleResourceString("Products.NewProducts")%></a> </li>
        <%} %>
        <li><a href="<%=Page.ResolveUrl("~/search.aspx")%>">
            <%=GetLocaleResourceString("Search.Search")%></a> </li>
        <li><a href="<%= SEOHelper.GetMyAccountUrl()%>">
            <%=GetLocaleResourceString("Account.MyAccount")%></a> </li>
        <% if (IoCFactory.Resolve<IBlogManager>().BlogEnabled)
           { %>
        <li><a href="<%= SEOHelper.GetBlogUrl()%>">
            <%=GetLocaleResourceString("Blog.Blog")%></a> </li>
        <%} %>
        <% if (IoCFactory.Resolve<IForumManager>().ForumsEnabled)
           { %>
        <li><a href="<%= SEOHelper.GetForumMainUrl()%>">
            <%=GetLocaleResourceString("Forum.Forums")%></a></li>
        <%} %>
        <li><a href="<%=Page.ResolveUrl("~/contactus.aspx")%>">
            <%=GetLocaleResourceString("ContactUs.ContactUs")%></a> </li>
    </ul>
</div>
