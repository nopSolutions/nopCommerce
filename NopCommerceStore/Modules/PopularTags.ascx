<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.PopularTagsControl"
    CodeBehind="PopularTags.ascx.cs" %>
<div class="block block-popular-tags">
    <div class="title">
        <%=GetLocaleResourceString("ProductTagsCloud.Title")%>
    </div>
    <div class="clear">
    </div>
    <div class="listbox">
        <asp:ListView ID="lvTagCloud" runat="server" EnableViewState="false">
            <LayoutTemplate>
                <div class="tags">
                    <ul>
                        <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                    </ul>
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <li><a style='font-size: <%# GetFontSize((int)Eval("ProductCount")) %>%' href="<%#CommonHelper.GetStoreLocation()%>producttag.aspx?tagid=<%#Eval("ProductTagId")%>">
                    <%#Server.HtmlEncode((string)Eval("Name"))%></a>&nbsp;&nbsp;</li>
            </ItemTemplate>
        </asp:ListView>
    </div>
</div>
