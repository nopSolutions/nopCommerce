UPDATE Store
SET Url = 'https://stage.abcwarehouse.com'
WHERE Url = 'https://www.abcwarehouse.com/'

UPDATE Store
SET Hosts = 'stage.abcwarehouse.com'
WHERE Hosts = 'www.abcwarehouse.com'

UPDATE Store
SET Url = 'https://stagehawthorne.abcwarehouse.com'
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
