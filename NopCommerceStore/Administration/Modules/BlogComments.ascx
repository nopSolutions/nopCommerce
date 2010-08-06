<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BlogCommentsControl"
    CodeBehind="BlogComments.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.BlogComments.Title")%>" />
    <%=GetLocaleResourceString("Admin.BlogComments.Title")%>
</div>
<asp:GridView ID="gvBlogComments" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvBlogComments_PageIndexChanging" AllowPaging="true" PageSize="10">
    <Columns>
        <asp:TemplateField ItemStyle-Width="100%" ItemStyle-HorizontalAlign="Left">
            <ItemTemplate>
                <p>
                    <%#BlogManager.FormatCommentText((string)Eval("CommentText"))%>
                </p>
                <p>
                    <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
                    -
                    <%#GetCustomerInfo(Convert.ToInt32(Eval("CustomerId")))%>
                    <%# string.Format(GetLocaleResourceString("Admin.BlogComments.IPAddress"), Eval("IPAddress"))%>
                </p>
                <p>
                    <a href="BlogPostDetails.aspx?BlogPostID=<%#Eval("BlogPostId")%>">
                        <%#Server.HtmlEncode(((BlogPost)Eval("BlogPost")).BlogPostTitle)%></a>
                </p>
                <asp:Button runat="server" ID="btnEditBlogComment" CssClass="adminButton" Text="<% $NopResources:Admin.BlogComments.EditButton.Text %>"
                    ToolTip="<% $NopResources:Admin.BlogComments.EditButton.Tooltip %>" CommandName="EditItem"
                    OnCommand="btnEditBlogComment_Click" CommandArgument='<%#Eval("BlogCommentId")%>' />
                <asp:Button runat="server" ID="btnDeleteBlogComment" CssClass="adminButton" Text="<% $NopResources:Admin.BlogComments.DeleteButton.Text %>"
                    ToolTip="<% $NopResources:Admin.BlogComments.DeleteButton.Tooltip %>" CommandName="DeleteItem"
                    OnCommand="btnDeleteBlogComment_Click" CommandArgument='<%#Eval("BlogCommentId")%>' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerSettings PageButtonCount="50" Position="TopAndBottom" />
</asp:GridView>
