param([String]$action="",[String]$websiteName="")

if (!$websiteName) { 
        Write-Host "variable is null" 
        BREAK}
		
if($action -eq "on" -or $action -eq "start")
{
	start-azurewebsite  -name $websiteName
	Write-Host "START " $websiteName 
}
elseif($action -eq "reset" -or $action -eq "restart")
{
	Restart-AzureWebsite -name $websiteName
	Write-Host "Restart " $websiteName 
}
else
{
	Restart-AzureWebsite -name $websiteName
	stop-azurewebsite -name $websiteName
	Write-Host "STOP " $websiteName 
}