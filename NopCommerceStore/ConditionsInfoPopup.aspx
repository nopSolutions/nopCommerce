<%@ Page Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.ConditionsInfoPopupPage"
    CodeBehind="ConditionsInfoPopup.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Topic" Src="~/Modules/Topic.ascx" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Terms Of Service</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <nopCommerce:Topic ID="topicConditionsInfo" runat="server" TopicName="ConditionsOfUse">
        </nopCommerce:Topic>
    </div>
    </form>
</body>
</html>
