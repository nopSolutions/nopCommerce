USE [NOPCommerce]
UPDATE Store
SET Url = 'http://127.0.0.1:5000/'
WHERE Url = 'https://www.abcwarehouse.com/'
OR Url = 'https://stage.abcwarehouse.com/'

UPDATE Store
SET SslEnabled = 0

UPDATE Setting
SET Value = 'WithoutWww'
WHERE Name = 'seosettings.wwwrequirement'

-- Minification
UPDATE Setting
SET Value = 'False'
WHERE Name = 'commonsettings.enablehtmlminification'
    OR Name = 'commonsettings.minificationenabled'
    OR Name = 'commonsettings.enablecssbundling'

UPDATE Setting
SET Value = 'True'
WHERE Name = 'coresettings.areexternalcallsskipped'

UPDATE Setting
SET Value = ''
WHERE Name = 'synchronypaymentsettings.merchantpassword'
    OR Name = 'storelocatorsettings.googleapikey'

UPDATE ScheduleTask
SET Enabled = 0

-- Geocoding
UPDATE Setting
SET Value = 'GEOCODING_API_KEY'
WHERE Name = 'coresettings.googlemapsgeocodingapikey'

-- Unifi
UPDATE Setting
SET Value = 'True'
WHERE Name = 'unifipaymentssettings.useintegration' or Name = 'unifisettings.useintegration'
UPDATE Setting
SET Value = 'CLIENT_ID'
WHERE Name = 'unifipaymentssettings.clientid'
UPDATE Setting
SET Value = 'CLIENT_SECRET'
WHERE Name = 'unifipaymentssettings.clientsecret'