<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.PricelistControl"
    CodeBehind="Pricelist.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-promotions.png" alt="<%=GetLocaleResourceString("Admin.Pricelists.Title")%>" />
        <%=GetLocaleResourceString("Admin.Pricelists.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='PricelistAdd.aspx'" value="<%=GetLocaleResourceString("Admin.Pricelists.AddNewButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.Pricelists.AddNewButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvPricelists" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvPricelists_PageIndexChanging" AllowPaging="true" PageSize="15"
    Visible="true">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Pricelists.Name %>" ItemStyle-Width="80%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("DisplayName").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Pricelists.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="PricelistDetails.aspx?PricelistID=<%#Eval("PricelistId")%>" title="<%#GetLocaleResourceString("Admin.Pricelists.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.Pricelists.Edit")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
