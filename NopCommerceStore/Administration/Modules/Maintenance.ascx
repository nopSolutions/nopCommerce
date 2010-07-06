<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.MaintenanceControl"
    CodeBehind="Maintenance.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-system.png" alt="<%=GetLocaleResourceString("Admin.Maintenance.Title")%>" />
        <%=GetLocaleResourceString("Admin.Maintenance.Title")%>
    </div>
    <div class="options">
        <asp:Button ID="BackupButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Maintenance.BackupButton.Text %>"
            OnClick="BackupButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.Maintenance.BackupButton.Tooltip %>" />
        <asp:Button ID="btnBackupPictures" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Maintenance.BtnBackupPictures.Text %>" OnClick="BtnBackupPictures_OnClick" CausesValidation="false" ToolTip="<% $NopResources:Admin.Maintenance.BtnBackupPictures.Tooltip %>" />
    </div>
</div>
<asp:GridView ID="gvBackups" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvBackups_PageIndexChanging" AllowPaging="true" PageSize="15">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Maintenance.FileNameColumn %>" ItemStyle-Width="40%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("FileName").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Maintenance.FileSizeColumn %>" ItemStyle-Width="20%">
            <ItemTemplate>
                <%# GetFileSizeInfo( (long)(Eval("FileSize")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Maintenance.RestoreColumn %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Button ID="RestoreButton" runat="server" CssClass="adminButton" CommandName="RestoreBackup"
                    Text="<% $NopResources:Admin.Maintenance.RestoreButton.Text %>" CommandArgument='<%#Eval("FullFileName")%>'
                    OnCommand="RestoreButton_OnCommand" CausesValidation="false" ToolTip="<% $NopResources:Admin.Maintenance.RestoreButton.Tooltip %>" />
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Maintenance.DeleteColumn %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Button ID="DeleteButton" runat="server" CssClass="adminButton" CommandName="DeleteBackup"
                    Text="<% $NopResources:Admin.Maintenance.DeleteButton.Text %>" CommandArgument='<%#Eval("FullFileName")%>'
                    OnCommand="DeleteButton_OnCommand" CausesValidation="false" ToolTip="<% $NopResources:Admin.Maintenance.DeleteButton.Tooltip %>" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
