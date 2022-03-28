# restore DB
sqlcmd -S localhost -U sa -P P@ssw0rd -i .devcontainer/restore-scripts/restoreDb-staging.sql