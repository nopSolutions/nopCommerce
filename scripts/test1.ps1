# You can write your azure powershell scripts inline here. 
# You can also pass predefined and custom variables to this script using arguments

param([String]$websiteName="",[string]$slotName)

Restart-AzureRmWebAppSlot -ResourceGroupName "Default-Web-WestEurope" -Name $websiteName -Slot "proda"