<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.PrivateMessagesSentItemsControl"
    CodeBehind="PrivateMessagesSentItems.ascx.cs" %>
<div class="private-messages-box">


<script type="text/javascript">

    $(window).bind('load', function () {
        var cbHeader = $(".cbHeaderPMSent input");
        var cbRowItem = $(".cbRowItemPMSent input");
        cbHeader.bind("click", function () {
            cbRowItem.each(function () { this.checked = cbHeader[0].checked; })
        });
        cbRowItem.bind("click", function () { if ($(this).checked == false) cbHeader[0].checked = false; });
    });
    
</script>

    <asp:GridView ID="gvSent" DataKeyNames="PrivateMessageId" runat="server" AllowPaging="True"
        AutoGenerateColumns="False" CellPadding="4" DataSourceID="odsSent" GridLines="None"
        PageSize="10" CssClass="pmgridtablestyle">
        <AlternatingRowStyle CssClass="pmgridaltrowstyle" />
        <HeaderStyle CssClass="pmgridheaderstyle" />
        <RowStyle CssClass="pmgridrowstyle" />
        <PagerStyle CssClass="pmgridpagerstyle" />
        <Columns>
            <asp:TemplateField ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <asp:CheckBox ID="cbSelectAll" runat="server" CssClass="cbHeaderPMSent" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="cbSelect" runat="server" CssClass="cbRowItemPMSent" />
                    <asp:HiddenField ID="hfPrivateMessageId" runat="server" Value='<%# Eval("PrivateMessageId") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:PrivateMessages.Sent.ToColumn %>"
                ItemStyle-Width="20%">
                <ItemTemplate>
                    <%#GetToInfo(Convert.ToInt32(Eval("ToUserId")))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:PrivateMessages.Sent.SubjectColumn %>"
                ItemStyle-Width="50%">
                <ItemTemplate>
                    <%#GetSubjectInfo(Container.DataItem as PrivateMessage)%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:PrivateMessages.Sent.DateColumn %>"
                HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="25%" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <div>
        <asp:ObjectDataSource ID="odsSent" runat="server" SelectMethod="GetCurrentUserSentPrivateMessages"
            EnablePaging="true" TypeName="NopSolutions.NopCommerce.Web.ForumHelper" StartRowIndexParameterName="StartIndex"
            MaximumRowsParameterName="PageSize" SelectCountMethod="GetCurrentUserSentPrivateMessagesCount">
        </asp:ObjectDataSource>
    </div>
    <div class="clear">
    </div>
    <div class="button">
        <asp:Button runat="server" ID="btnDeleteSelected" Text="<% $NopResources:PrivateMessages.Sent.DeleteSelected %>"
            ValidationGroup="SentPrivateMessages" OnClick="btnDeleteSelected_Click" CssClass="deleteselectedpmbutton">
        </asp:Button>
    </div>
</div>
