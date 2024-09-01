USE [NOPCommerce]
UPDATE Store
SET Url = 'https://stage.abcwarehouse.com'
WHERE Url = 'https://www.abcwarehouse.com/'

UPDATE Store
SET Url = 'https://stagehawthorne.abcwarehouse.com'
WHERE Url = 'https://hawthorne.abcwarehouse.com/'

UPDATE Setting
SET Value = 'WithoutWww'
WHERE Name = 'seosettings.wwwrequirement'

UPDATE Setting
SET Value = ''
WHERE Name = 'synchronypaymentsettings.merchantpassword'
    OR Name = 'storelocatorsettings.googleapikey'

UPDATE EmailAccount
SET Host = 'email.abcwarehouse.com'
