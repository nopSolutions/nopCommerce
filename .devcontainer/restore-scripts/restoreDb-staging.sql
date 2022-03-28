RESTORE DATABASE [StagingDb] FROM DISK = N'/var/opt/mssql/backup/StagingDb.bak'
WITH FILE = 1, NOUNLOAD, REPLACE, STATS = 5,
MOVE 'StagingDb' to '/var/opt/mssql/data/StagingDb.mdf',
MOVE 'StagingDb_log' to '/var/opt/mssql/data/StagingDb_Log.ldf'
