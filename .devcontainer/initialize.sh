echo 'Initializing gcloud CLI...'
echo
gcloud init
echo 'Downloading database from Cloud Storage...'
echo
gcloud storage cp gs://dfar-storage/NOP.bacpac .\NOP.bacpac
echo 'Importing database...'
echo
/opt/sqlpackage/sqlpackage /Action:Import /SourceFile:"/workspace/.NOP.bacpac" /TargetConnectionString:"Server=tcp:localhost,1433;Initial Catalog=NOPCommerce;User ID=sa;Password=P@ssw0rd;TrustServerCertificate=True;"
echo 'Copying settings files'
echo
gcloud storage cp gs://dfar-storage/appsettings.json src/Presentation/Nop.Web/App_Data/
gcloud storage cp gs://dfar-storage/plugins.json src/Presentation/Nop.Web/App_Data/
cp .devcontainer/dataSettings.json src/Presentation/Nop.Web/App_Data/dataSettings.json
echo 'Cleaning up...'
echo
rm .NOP.bacpac