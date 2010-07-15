<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.NewsletterSubscribersControl"
    CodeBehind="NewsletterSubscribers.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-promotions.png" alt="<%=GetLocaleResourceString("Admin.NewsletterSubscribers.Title")%>" />
        <%=GetLocaleResourceString("Admin.NewsletterSubscribers.Title")%>
    </div>
    <div class="options">
        <asp:Button ID="SearchButton" runat="server" Text="<% $NopResources:Admin.NewsletterSubscribers.SearchButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SearchButton_Click" />
        <asp:Button runat="server" Text="<% $NopResources:Admin.NewsletterSubscribers.ImportEmailsButton.Text %>"
            CssClass="adminButtonBlue" ID="btnImportCSV" OnClick="btnImportCSV_Click" 
            ToolTip="<% $NopResources:Admin.NewsletterSubscribers.ImportEmailsButton.Tooltip %>" />
       <asp:Button runat="server" Text="<% $NopResources:Admin.NewsletterSubscribers.ExportEmailsButton.Text %>"
            CssClass="adminButtonBlue" ID="btnExportCVS" OnClick="btnExportCVS_Click"
            ToolTip="<% $NopResources:Admin.NewsletterSubscribers.ExportEmailsButton.Tooltip %>" />
        <asp:Button runat="server" Text="<% $NopResources:Admin.NewsletterSubscribers.DeleteButton.Text %>"
            CssClass="adminButtonBlue" ID="btnDelete" OnClick="btnDelete_Click" />
        <nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="btnDelete"
            YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
            ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
    </div>
</div>
<table width="100%">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblEmail" Text="<% $NopResources:Admin.NewsletterSubscribers.Email %>"
                ToolTip="<% $NopResources:Admin.NewsletterSubscribers.Email.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtEmail" CssClass="adminInput" runat="server"></asp:TextBox>
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
<asp:GridView ID="gvNewsletterSubscribers" runat="server" AutoGenerateColumns="False"
    Width="100%" OnPageIndexChanging="gvNewsletterSubscribers_PageIndexChanging"
    AllowPaging="true" PageSize="15">
    <Columns>
        <asp:TemplateField ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <asp:CheckBox ID="cbSelectAll" runat="server" CssClass="cbHeader" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="cbNewsLetterSubscription" runat="server" CssClass="cbRowItem" />
                <asp:HiddenField ID="hfNewsLetterSubscriptionId" runat="server" Value='<%# Eval("NewsLetterSubscriptionId") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.NewsletterSubscribers.EmailColumn %>"
            ItemStyle-Width="50%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Email").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.NewsletterSubscribers.Active %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:ImageCheckBox runat="server" ID="cbActive" Checked='<%# Eval("Active") %>'>
                </nopCommerce:ImageCheckBox>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.NewsletterSubscribers.SubscribedOnColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerSettings PageButtonCount="50" Position="TopAndBottom" />
</asp:GridView>
<ajaxToolkit:ConfirmButtonExtender ID="cbeImportCSV" runat="server" TargetControlID="btnImportCSV"
    DisplayModalPopupID="mpeImportCSV" />
<ajaxToolkit:ModalPopupExtender runat="server" ID="mpeImportCSV" TargetControlID="btnImportCSV"
    OkControlID="btnImportCSVOk" CancelControlID="btnImportCSVCancel" PopupControlID="pnlImportCSVPopupPanel"
    BackgroundCssClass="modalBackground" />
<asp:Panel runat="server" ID="pnlImportCSVPopupPanel" Style="display: none; width: 250px;
    background-color: White; border-width: 2px; border-color: Black; border-style: solid;
    padding: 20px;">
    <div style="text-align: center;">
        <%=GetLocaleResourceString("Admin.NewsletterSubscribers.ImportCSV.CSVFile")%>
        <asp:FileUpload runat="server" ID="fuCsvFile" />
        <asp:Button ID="btnImportCSVOk" runat="server" Text="<% $NopResources:Admin.Common.OK %>"
            CssClass="adminButton" CausesValidation="false" />
        <asp:Button ID="btnImportCSVCancel" runat="server" Text="<% $NopResources:Admin.Common.Cancel %>"
            CssClass="adminButton" CausesValidation="false" />
    </div>
</asp:Panel>
