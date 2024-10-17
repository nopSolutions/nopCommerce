UPDATE Store
SET Url = 'https://stage.abcwarehouse.com/'
WHERE Url = 'https://www.abcwarehouse.com/'

UPDATE Store
SET Hosts = 'stage.abcwarehouse.com'
WHERE Hosts = 'www.abcwarehouse.com'

UPDATE Store
SET Url = 'https://stagehawthorne.abcwarehouse.com/'
WHERE Url = 'https://hawthorne.abcwarehouse.com/'

UPDATE Store
SET Hosts = 'stagehawthorne.abcwarehouse.com'
WHERE Hosts = 'hawthorne.abcwarehouse.com'

UPDATE Setting
SET Value = 'WithoutWww'
WHERE Name = 'seosettings.wwwrequirement'

UPDATE Setting
SET Value = 'TS2_'
WHERE Name = 'exportordersettings.tableprefix'

UPDATE EmailAccount
SET Host = 'email.abcwarehouse.com'

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

-- Minification
UPDATE Setting
SET Value = 'False'
WHERE Name = 'commonsettings.enablehtmlminification'
    OR Name = 'commonsettings.minificationenabled'
    OR Name = 'commonsettings.enablecssbundling'