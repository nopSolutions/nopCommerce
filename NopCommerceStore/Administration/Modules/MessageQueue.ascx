<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.MessageQueueControl" CodeBehind="MessageQueue.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DatePicker" Src="DatePicker.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="Common/ico-system.png" alt="<%=GetLocaleResourceString("Admin.MessageQueue.Title")%>" />
        <%=GetLocaleResourceString("Admin.MessageQueue.Title")%>
    </div>
    <div class="options">
        <asp:Button ID="LoadButton" runat="server" Text="<% $NopResources:Admin.MessageQueue.LoadButton.Text %>"
            CssClass="adminButtonBlue" OnClick="LoadButton_Click" ToolTip="<% $NopResources:Admin.MessageQueue.LoadButton.Tooltip %>" />
        <asp:Button runat="server" Text="<% $NopResources:Admin.MessageQueue.DeleteButton.Text %>"
            CssClass="adminButtonBlue" ID="btnDelete" OnClick="btnDelete_Click" />
    </div>
</div>
<table width="100%" class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblStartDate" Text="<% $NopResources:Admin.MessageQueue.StartDate %>"
                ToolTip="<% $NopResources:Admin.MessageQueue.StartDate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlStartDatePicker" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblEndDate" Text="<% $NopResources:Admin.MessageQueue.EndDate %>"
                ToolTip="<% $NopResources:Admin.MessageQueue.EndDate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlEndDatePicker" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblFromEmail" Text="<% $NopResources:Admin.MessageQueue.From %>"
                ToolTip="<% $NopResources:Admin.MessageQueue.From.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtFromEmail" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblToEmail" Text="<% $NopResources:Admin.MessageQueue.ToEmail %>"
                ToolTip="<% $NopResources:Admin.MessageQueue.ToEmail.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtToEmail" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblLoadNotSentItemsOnly" Text="<% $NopResources:Admin.MessageQueue.LoadNotSent %>"
                ToolTip="<% $NopResources:Admin.MessageQueue.LoadNotSent.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbLoadNotSentItemsOnly" runat="server" Checked="true"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblMaxSendTries" Text="<% $NopResources:Admin.MessageQueue.MaxSendTries %>"
                ToolTip="<% $NopResources:Admin.MessageQueue.MaxSendTries.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtMaxSendTries"
                RequiredErrorMessage="<% $NopResources:Admin.MessageQueue.MaxSendTries.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" Value="10" RangeErrorMessage="<% $NopResources:Admin.MessageQueue.MaxSendTries.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblGoDirectlyToEmailNumber" Text="<% $NopResources:Admin.MessageQueue.GoDirectly %>"
                ToolTip="<% $NopResources:Admin.MessageQueue.GoDirectly.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtEmailId" Width="100px"
                ValidationGroup="GoDirectly" ErrorMessage="<% $NopResources:Admin.MessageQueue.EmailID.Required %>">
            </nopCommerce:SimpleTextBox>
            <asp:Button runat="server" Text="<% $NopResources:Admin.MessageQueue.GoButton.Text %>"
                CssClass="adminButtonBlue" ID="btnGoDirectlyToEmailNumber" OnClick="btnGoDirectlyToEmailNumber_Click"
                ValidationGroup="GoDirectly" ToolTip="<% $NopResources:Admin.MessageQueue.GoButton.Tooltip %>" />
        </td>
    </tr>
</table>
<p>
</p>
<script type="text/javascript">

    $(window).bind('load', function () {
        var cbHeader = $(".cbHeader input");
        var cbRowItem = $(".cbRowItem input");
        cbHeader.bind("click", function () {
            cbRowItem.each(function () { this.checked = cbHeader[0].checked; })
        });
        cbRowItem.bind("click", function () { if ($(this).checked == false) cbHeader[0].checked = false; });
    });
    
</script>

<asp:GridView ID="gvQueuedEmails" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvQueuedEmails_PageIndexChanging" AllowPaging="true" PageSize="15">
    <Columns>
        <asp:TemplateField ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <asp:CheckBox ID="cbSelectAll" runat="server" CssClass="cbHeader" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="cbQueuedEmail" runat="server" CssClass="cbRowItem" />
                <asp:HiddenField ID="hfQueuedEmailId" runat="server" Value='<%# Eval("QueuedEmailId") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="QueuedEmailId" HeaderText="<% $NopResources:Admin.MessageQueue.QueuedEmailIDColumn %>"
            ItemStyle-Width="10%"></asp:BoundField>
        <asp:BoundField DataField="Priority" HeaderText="<% $NopResources:Admin.MessageQueue.PriorityColumn %>"
            ItemStyle-Width="5%"></asp:BoundField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.MessageQueue.FromColumn %>"
            ItemStyle-Width="20%">
            <ItemTemplate>
                <%#GetFromInfo(Container.DataItem as QueuedEmail)%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.MessageQueue.ToColumn %>" ItemStyle-Width="20%">
            <ItemTemplate>
                <%#GetToInfo(Container.DataItem as QueuedEmail)%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.MessageQueue.CreatedOnColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.MessageQueue.SentOnColumn %>"
            ItemStyle-Width="15%">
            <ItemTemplate>
                <%#GetSentOnInfo(Container.DataItem as QueuedEmail)%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.MessageQueue.ViewColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="MessageQueueDetails.aspx?QueuedEmailID=<%#Eval("QueuedEmailId")%>">
                    <%#GetLocaleResourceString("Admin.MessageQueue.ViewColumn")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerSettings PageButtonCount="50" Position="TopAndBottom" />
</asp:GridView>
<br />
<asp:Label runat="server" ID="lblQueuedEmailsFound" Text="<% $NopResources:Admin.MessageQueue.NoEmailsFound %>"
    Visible="false"></asp:Label>