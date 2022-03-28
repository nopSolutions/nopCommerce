RESTORE DATABASE [NOPCommerce] FROM DISK = N'/var/opt/mssql/backup/NOPCommerce.bak'
WITH FILE = 1, NOUNLOAD, REPLACE, STATS = 5,
MOVE 'NOPCommerce_Sample' to '/var/opt/mssql/data/NOPCommerce_Sample.mdf',
MOVE 'NOPCommerce_Sample_log' to '/var/opt/mssql/data/NOPCommerce_Sample_Log.ldf'
