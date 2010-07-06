<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BlogControl"
    CodeBehind="Blog.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.Blog.Title")%>" />
        <%=GetLocaleResourceString("Admin.Blog.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='BlogPostAdd.aspx'" value="<%=GetLocaleResourceString("Admin.Blog.AddNewButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.Blog.AddNewButton.Tooltip")%>" />
    </div>
</div>
<p>
</p>
<asp:GridView ID="gvBlogPosts" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvBlogPosts_PageIndexChanging" AllowPaging="true" PageSize="15">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Blog.BlogPostTitle %>" ItemStyle-Width="40%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("BlogPostTitle").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Blog.Language %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#((Language)Eval("Language")).Name%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Blog.ViewComments %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="BlogComments.aspx?BlogPostID=<%#Eval("BlogPostId")%>" title="<%#GetLocaleResourceString("Admin.Blog.ViewComments.Tooltip")%>">
                    <%#String.Format(GetLocaleResourceString("Admin.Blog.ViewComments.Link"), ((ICollection<BlogComment>)Eval("BlogComments")).Count)%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Blog.CreatedOn %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Blog.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="BlogPostDetails.aspx?BlogPostID=<%#Eval("BlogPostId")%>" title="<%#GetLocaleResourceString("Admin.Blog.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.Blog.Edit")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
