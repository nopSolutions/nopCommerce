

# SET Environment for SSL 

```bash
#ASPNETCORE_URLS=https://localhost:5001\;http://localhost:5000
export ASPNETCORE_URLS="https://localhost:5001;http://localhost:5000"
```

# Install Local Maria DB 

```bash
# 1. Update your package lists
sudo apt update

# 2. Install the MariaDB server package
sudo apt install mariadb-server

# 3. Secure your installation (set root password, remove test users, etc.)
sudo mysql_secure_installation

# 4. (Optional) Check MariaDB status
sudo systemctl status mariadb

```
