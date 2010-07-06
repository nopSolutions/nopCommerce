<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ForumControl"
    CodeBehind="Forums.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.Forums.Title")%>" />
        <%=GetLocaleResourceString("Admin.Forums.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='ForumGroupAdd.aspx'" value="<%=GetLocaleResourceString("Admin.Forums.ForumGroupAdd.Text")%>"
            id="btnAddNewForumGroup" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.Forums.ForumGroupAdd.Tooltip")%>" />
        <asp:Button runat="server" Text="<% $NopResources:Admin.Forums.AddNewForum.Text %>"
            CssClass="adminButtonBlue" ID="btnAddNewForum" OnClick="btnAddNewForum_Click"
            ToolTip="<% $NopResources:Admin.Forums.AddNewForum.Tooltip %>" />
    </div>
</div>
<p>
</p>
<asp:Repeater ID="rptrForumGroups" runat="server" OnItemDataBound="rptrForumGroups_ItemDataBound">
    <HeaderTemplate>
        <table class="forumtable">
            <tr class="headerstyle">
                <th>
                    <%=GetLocaleResourceString("Admin.Forums.NameColumn")%>
                </th>
                <th>
                    <%=GetLocaleResourceString("Admin.Forums.DisplayOrderColumn")%>
                </th>
                <th>
                    <%=GetLocaleResourceString("Admin.Forums.CreatedOnColumn")%>
                </th>
                <th>
                    <%=GetLocaleResourceString("Admin.Forums.EditColumn")%>
                </th>
            </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="forumgroup">
            <td class="name">
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </td>
            <td>
                <%#Eval("DisplayOrder")%>
            </td>
            <td>
                <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
            </td>
            <td>
                <a href="ForumGroupDetails.aspx?ForumGroupID=<%#Eval("ForumGroupId")%>">
                    <%=GetLocaleResourceString("Admin.Forums.EditColumn.ForumGroup")%>
                </a>
            </td>
        </tr>
        <asp:Repeater ID="rptrForums" runat="server">
            <ItemTemplate>
                <tr class="forum">
                    <td class="name">
                        <%#Server.HtmlEncode(Eval("Name").ToString())%>
                    </td>
                    <td>
                        <%#Eval("DisplayOrder")%>
                    </td>
                    <td>
                        <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
                    </td>
                    <td>
                        <a href="ForumDetails.aspx?ForumID=<%#Eval("ForumId")%>">
                            <%=GetLocaleResourceString("Admin.Forums.EditColumn.Forum")%>
                        </a>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>
