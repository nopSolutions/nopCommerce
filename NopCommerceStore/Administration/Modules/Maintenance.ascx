<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.MaintenanceControl"
    CodeBehind="Maintenance.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-system.png" alt="<%=GetLocaleResourceString("Admin.Maintenance.Title")%>" />
        <%=GetLocaleResourceString("Admin.Maintenance.Title")%>
    </div>
    <div class="options">
        <asp:Button ID="btnBackupButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Maintenance.BackupButton.Text %>"
            OnClick="btnBackupButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.Maintenance.BackupButton.Tooltip %>" />
        <asp:Button ID="btnBackupPictures" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Maintenance.BtnBackupPictures.Text %>"
            OnClick="btnBackupPictures_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.Maintenance.BtnBackupPictures.Tooltip %>" />
        <asp:Button ID="btnDeleteOldExportedFiles" runat="server" CssClass="adminButtonBlue"
            Text="<% $NopResources:Admin.Maintenance.DeleteOldExportedFilesButton.Text %>" OnClick="btnDeleteOldExportedFiles_Click"
            CausesValidation="false" ToolTip="<% $NopResources:Admin.Maintenance.DeleteOldExportedFilesButton.Tooltip %>" />
    </div>
</div>
<asp:GridView ID="gvBackups" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvBackups_PageIndexChanging" OnRowDataBound="gvBackups_RowDataBound" AllowPaging="true" PageSize="15">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Maintenance.FileNameColumn %>" ItemStyle-Width="40%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("FileName").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Maintenance.FileSizeColumn %>" ItemStyle-Width="15%">
            <ItemTemplate>
                <%# GetFileSizeInfo( (long)(Eval("FileSize")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Maintenance.DownloadColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:HyperLink ID="hlDownload" runat="server" Text="<% $NopResources:Admin.Maintenance.DownloadButton.Text %>"
                    CausesValidation="false" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Maintenance.RestoreColumn %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Button ID="RestoreButton" runat="server" CssClass="adminButton" CommandName="RestoreBackup"
                    Text="<% $NopResources:Admin.Maintenance.RestoreButton.Text %>" CommandArgument='<%#Eval("FullFileName")%>'
                    OnCommand="RestoreButton_OnCommand" CausesValidation="false" ToolTip="<% $NopResources:Admin.Maintenance.RestoreButton.Tooltip %>" />
            </ItemTemplate>
        </asp:TemplateField>        
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Maintenance.DeleteColumn %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Button ID="DeleteButton" runat="server" CssClass="adminButton" CommandName="DeleteBackup"
                    Text="<% $NopResources:Admin.Maintenance.DeleteButton.Text %>" CommandArgument='<%#Eval("FullFileName")%>'
                    OnCommand="DeleteButton_OnCommand" CausesValidation="false" ToolTip="<% $NopResources:Admin.Maintenance.DeleteButton.Tooltip %>" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
