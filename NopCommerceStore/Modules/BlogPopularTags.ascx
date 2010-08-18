<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.BlogPopularTagsControl"
    CodeBehind="BlogPopularTags.ascx.cs" %>
<div class="block block-popular-blogtags">
    <div class="title">
        <%=GetLocaleResourceString("Blog.TagsCloud.Title")%>
    </div>
    <div class="clear">
    </div>
    <div class="listbox">

     <asp:Repeater ID="rptrTagCloud" runat="server" OnItemDataBound="rptrTagCloud_ItemDataBound" EnableViewState="false">
            <HeaderTemplate>
                <div class="tags">
                <ul>
            </HeaderTemplate>
            <ItemTemplate>
                <li>
                    <asp:HyperLink ID="hlLink" runat="server" Text='<%#Server.HtmlEncode(Eval("Name").ToString()) %>' />
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
                </div>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</div>
