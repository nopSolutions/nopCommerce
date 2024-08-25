USE [NOPCommerce]
UPDATE Store
SET Url = 'http://localhost:5000/'
WHERE Url = 'https://www.abcwarehouse.com/'
OR Url = 'https://stage.abcwarehouse.com/'

UPDATE Store
SET SslEnabled = 0

UPDATE Setting
SET Value = 'WithoutWww'
WHERE Name = 'seosettings.wwwrequirement'

UPDATE Setting
SET Value = 'False'
WHERE Name = 'commonsettings.enablehtmlminification'
    OR Name = 'commonsettings.minificationenabled'

UPDATE Setting
SET Value = 'True'
WHERE Name = 'coresettings.areexternalcallsskipped'

UPDATE Setting
SET Value = ''
WHERE Name = 'synchronypaymentsettings.merchantpassword'
    OR Name = 'storelocatorsettings.googleapikey'

UPDATE ScheduleTask
SET Enabled = 0

UPDATE Setting
SET Value = 'AIzaSyDUDi-Nroi5soRN8d41Kf2Tr7t_aE-TNGg'
WHERE Name = 'coresettings.googlemapsgeocodingapikey'