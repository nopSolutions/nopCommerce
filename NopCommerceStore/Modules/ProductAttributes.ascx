<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProductAttributesControl" Codebehind="ProductAttributes.ascx.cs" %>

<% if(SettingManager.GetSettingValueBoolean("ProductAttribute.EnableDynamicPriceUpdate")) { %>
<script type="text/javascript">
    // Adjustment table
    var <%=AdjustmentTableName%> = new Array();
    // Adjustment table initialize
    <asp:Literal runat="server" ID="lblAdjustmentTableScripts" />
    // Adjustment function
    function <%=AdjustmentFuncName%>() {
        var sum = 0;
        for(var i in <%=AdjustmentTableName%>) {
            var ctrl = $('#' + i);
            if((ctrl.is(':radio') && ctrl.is(':checked')) || (ctrl.is(':checkbox') && ctrl.is(':checked'))) {
                sum += <%=AdjustmentTableName%>[i];
            }
            else if(ctrl.is('select')) {
                var idx = $('#' + i + " option").index($('#' + i + " option:selected"));
                if(idx != -1) {
                    sum += <%=AdjustmentTableName%>[i][idx];
                }
            }
        }
        var res = (<%=PriceVarName%> + sum).toFixed(2);
        $(".<%=PriceVarClass%>").text(res);
    }
    // Attributes handlers
    $(document).ready(function() {
        <%=AdjustmentFuncName%>();
        <asp:Literal runat="server" ID="lblAttributeScripts" />
    });
</script>
<%} %>
<asp:PlaceHolder runat="server" ID="phAttributes" />
