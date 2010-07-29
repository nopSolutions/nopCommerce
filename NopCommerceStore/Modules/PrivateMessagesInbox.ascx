<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.PrivateMessagesInboxControl"
    CodeBehind="PrivateMessagesInbox.ascx.cs" %>
<div class="private-messages-box">
    <script type="text/javascript">

        $(window).bind('load', function () {
            var cbHeader = $(".cbHeaderPMInbox input");
            var cbRowItem = $(".cbRowItemPMInbox input");
            cbHeader.bind("click", function () {
                cbRowItem.each(function () { this.checked = cbHeader[0].checked; })
            });
            cbRowItem.bind("click", function () { if ($(this).checked == false) cbHeader[0].checked = false; });
        });
    
    </script>
    <div class="PrivateMessages">
        <asp:GridView ID="gvInbox" DataKeyNames="PrivateMessageId" runat="server" AllowPaging="True"
            AutoGenerateColumns="False" CellPadding="4" DataSourceID="odsInbox" GridLines="None"
            PageSize="10" CssClass="pmgridtablestyle">
            <AlternatingRowStyle CssClass="pmgridaltrowstyle" />
            <HeaderStyle CssClass="pmgridheaderstyle" />
            <RowStyle CssClass="pmgridrowstyle" />
            <PagerStyle CssClass="pmgridpagerstyle" />
            <Columns>
                <asp:TemplateField ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
                    <HeaderTemplate>
                        <asp:CheckBox ID="cbSelectAll" runat="server" CssClass="cbHeaderPMInbox" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="cbSelect" runat="server" CssClass="cbRowItemPMInbox" />
                        <asp:HiddenField ID="hfPrivateMessageId" runat="server" Value='<%# Eval("PrivateMessageId") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<% $NopResources:PrivateMessages.Inbox.FromColumn %>"
                    ItemStyle-Width="20%">
                    <ItemTemplate>
                        <%#GetFromInfo(Convert.ToInt32(Eval("FromUserId")))%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<% $NopResources:PrivateMessages.Inbox.SubjectColumn %>"
                    ItemStyle-Width="50%">
                    <ItemTemplate>
                        <%#GetSubjectInfo(Container.DataItem as PrivateMessage)%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="<% $NopResources:PrivateMessages.Inbox.DateColumn %>"
                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="25%" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    <div>
        <asp:ObjectDataSource ID="odsInbox" runat="server" SelectMethod="GetCurrentUserInboxPrivateMessages"
            EnablePaging="true" TypeName="NopSolutions.NopCommerce.Web.ForumHelper" StartRowIndexParameterName="StartIndex"
            MaximumRowsParameterName="PageSize" SelectCountMethod="GetCurrentUserInboxPrivateMessagesCount">
        </asp:ObjectDataSource>
    </div>
    <div class="clear">
    </div>
    <div class="button">
        <asp:Button runat="server" ID="btnDeleteSelected" Text="<% $NopResources:PrivateMessages.Inbox.DeleteSelected %>"
            ValidationGroup="InboxPrivateMessages" OnClick="btnDeleteSelected_Click" CssClass="deleteselectedpmbutton">
        </asp:Button>
        <asp:Button runat="server" ID="btnMarkAsUnread" Text="<% $NopResources:PrivateMessages.Inbox.MarkAsUnread %>"
            ValidationGroup="InboxPrivateMessages" OnClick="btnMarkAsUnread_Click" CssClass="deleteselectedpmbutton">
        </asp:Button>
    </div>
</div>
