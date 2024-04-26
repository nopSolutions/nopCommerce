Version specific steps:

1. UPS is changing its API from access keys to OAuth starting June 3, 2024. So we've updated our UPS plugin accordingly. All existing users of this plugin have to reconfigure the plugin with OAuth settings. Please find more information at https://developer.ups.com/oauth-developer-guide

2. Default values are set for "Bundling & minimization". If you had non-default values, please re-configure "Bundling & minimization" in Admin area > Configuration > Settings > App settings (or in the \App_Data\appsettings.json file).