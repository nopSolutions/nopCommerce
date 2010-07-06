<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductTagsControl" CodeBehind="ProductTags.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SelectCategoryControl" Src="SelectCategoryControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.ProductTags.Title")%>" />
        <%=GetLocaleResourceString("Admin.ProductTags.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.ProductTags.DeleteButton.Text %>"
            CssClass="adminButtonBlue" ID="btnDelete" OnClick="btnDelete_Click" />
            <nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="btnDelete"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
    </div>
</div>
<p>
</p>
<script type="text/javascript">

    $(window).bind('load', function() {
        var cbHeader = $(".cbHeader input");
        var cbRowItem = $(".cbRowItem input");
        cbHeader.bind("click", function() {
            cbRowItem.each(function() { this.checked = cbHeader[0].checked; })
        });
        cbRowItem.bind("click", function() { if ($(this).checked == false) cbHeader[0].checked = false; });
    });
    
</script>
<asp:GridView ID="gvProductTags" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvProductTags_PageIndexChanging" AllowPaging="true" PageSize="15">
    <Columns>
        <asp:TemplateField ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <asp:CheckBox ID="cbSelectAll" runat="server" CssClass="cbHeader" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="cbProductTag" runat="server" CssClass="cbRowItem" />
                <asp:HiddenField ID="hfProductTagId" runat="server" Value='<%# Eval("ProductTagId") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductTags.Name %>" ItemStyle-Width="70%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductTags.Count %>" ItemStyle-Width="20%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("ProductCount").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<br />
<asp:Label runat="server" ID="lblNoProductTags" Text="<% $NopResources: Admin.Products.NoProductTags%>"
    Visible="false"></asp:Label>
