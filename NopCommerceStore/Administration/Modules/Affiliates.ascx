<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.AffiliatesControl"
    CodeBehind="Affiliates.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-promotions.png" alt="<%=GetLocaleResourceString("Admin.Affiliates.Title")%>" />
        <%=GetLocaleResourceString("Admin.Affiliates.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='AffiliateAdd.aspx'" value="<%=GetLocaleResourceString("Admin.Affiliates.AddNewButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.Affiliates.AddNewButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvAffiliates" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Affiliates.Name %>" ItemStyle-Width="60%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("FullName").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Affiliates.Active %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="25%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:ImageCheckBox runat="server" ID="cbPublished" Checked='<%# Eval("Active") %>'>
                </nopCommerce:ImageCheckBox>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Affiliates.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="AffiliateDetails.aspx?AffiliateID=<%#Eval("AffiliateId")%>" title="<%#GetLocaleResourceString("Admin.Affiliates.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.Affiliates.Edit")%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
