#!/bin/sh

cp ./.devcontainer/plugins.json ./src/Presentation/Nop.Web/App_Data/plugins.json

cp ./.devcontainer/dataSettings.json.template ./src/Presentation/Nop.Web/App_Data/dataSettings.json
read -p "SQL IP Address: " ip_address
read -s -p "Password: " password
sed -i "s/{IP_ADDRESS}/$ip_address/g" ./src/Presentation/Nop.Web/App_Data/dataSettings.json
sed -i "s/{PASSWORD}/$password/g" ./src/Presentation/Nop.Web/App_Data/dataSettings.json