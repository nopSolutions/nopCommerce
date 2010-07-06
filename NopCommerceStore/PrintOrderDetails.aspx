<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.PrintOrderDetails" Theme="Print" Codebehind="PrintOrderDetails.aspx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="OrderDetails" Src="~/Modules/OrderDetails.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Print Order Details</title>
    <script type="text/javascript">
        function printPage() 
        {
            window.print()
        }
    </script>
</head>
<body onload="javascript:printPage()">
    <form id="form1" runat="server">
    <div>
        <nopCommerce:OrderDetails ID="ctrlOrderDetails" runat="server" IsInvoice="true" />
    </div>
    </form>
</body>
</html>
