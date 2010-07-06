<%@ Application Language="C#" %>
<%@ Import Namespace="NopSolutions.NopCommerce.BusinessLogic.Configuration" %>
<%@ Import Namespace="NopSolutions.NopCommerce.BusinessLogic" %>
<%@ Import Namespace="NopSolutions.NopCommerce.BusinessLogic.Installation" %>
<%@ Import Namespace="NopSolutions.NopCommerce.BusinessLogic.Utils" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.IO" %>

<script runat="server">

    void Application_BeginRequest(object sender, EventArgs e)
    {
        NopConfig.Init();
        if (!InstallerHelper.ConnectionStringIsSet())
        {
            InstallerHelper.RedirectToInstallationPage();
        }
    }


    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        NopConfig.Init();
        if (InstallerHelper.ConnectionStringIsSet())
        {
            TaskManager.Instance.Initialize(NopConfig.ScheduleTasks);
            TaskManager.Instance.Start();
        }
    }
    
    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown
        if (InstallerHelper.ConnectionStringIsSet())
        {
            TaskManager.Instance.Stop();
        }
    }
    
    void Application_Error(object sender, EventArgs e)
    {
        Exception ex = Server.GetLastError();
        if (ex != null)
        {
            if (InstallerHelper.ConnectionStringIsSet())
            {
                LogManager.InsertLog(LogTypeEnum.Unknown, ex.Message, ex);
            }
        }
    }
    
</script>

