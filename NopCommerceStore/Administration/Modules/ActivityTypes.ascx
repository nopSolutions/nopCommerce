<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ActivityTypesControl"
    CodeBehind="ActivityTypes.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.ActivityLogType.Title")%>" />
        <%=GetLocaleResourceString("Admin.ActivityLogType.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.ActivityLogType.SaveButton.Text %>"
            CssClass="adminButtonBlue" ID="btnSave" OnClick="btnSave_Click" ToolTip="<% $NopResources:Admin.ActivityLogType.SaveButton.Tooltip %>" />
    </div>
</div>
<table class="adminContent">
    <tr>
        <td>

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

            <asp:GridView ID="gvActivityTypes" runat="server" AutoGenerateColumns="False" Width="100%">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="<% $NopResources:Admin.ActivityLogType.Name %>" />
                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                        <HeaderTemplate>
                            <div style="float: left;">
                                <%=GetLocaleResourceString("Admin.ActivityLogType.Enabled")%></div>
                            <div style="float: right;">
                                <asp:CheckBox ID="cbSelectAll" runat="server" CssClass="cbHeader" />
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:HiddenField runat="server" ID="hfActivityLogTypeId" Value='<%#Eval("ActivityLogTypeId")%>' />
                            <asp:CheckBox ID="cbEnabled" runat="server" Checked='<%#Eval("Enabled")%>' CssClass="cbRowItem" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
        </td>
    </tr>
</table>
