# UsefulWebApps

**UsefulWebApps** is an **ASP.NET Core MVC** application with a **MySQL database** backend.

- **User authentication** is handled via [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-8.0&tabs=visual-studio) with [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) for database access.  
- **Other data objects** are mapped using [Dapper](https://github.com/DapperLib/Dapper).  
- The app implements the **Repository and Unit of Work pattern** to separate data access logic from controllers and business logic.  

The application is hosted on an **Ubuntu 24.04 server** behind an **Nginx reverse proxy**. The deployment is **self-contained**, so no .NET runtime installation is required on the server. Useful setup documentation [here](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-8.0&tabs=linux-ubuntu) and [here](https://www.digitalocean.com/community/tutorials/how-to-deploy-an-asp-net-core-application-with-mysql-server-using-nginx-on-ubuntu-18-04). 

## Purpose

The goal of **UsefulWebApps** is to provide an open-source, self-hosted **family organizer** application.
Current functionality includes:

- Recipes management
- Lists
- Weather
- User-customized home pages

Future plans include:

- Family calendar
- Family chat
- Website aggregator
- Optional email server for enhanced login and notifications

---

## Local Development Setup

1. Install **MySQL Community Server** (v8.0.37 used in development).  
2. Clone the repository and build the project with **Visual Studio**.  
3. **Database seeding**: The plan is to provide a development MySQL dump with dummy users and data. You can log in using the provided admin password, then update it before backing up the database and hosting it on your own server.
4. Run the application locally to test functionality. 

---
## Backup and Restore on Windows

1. Open **Command Prompt** (not PowerShell) in MySql Server bin
```
cd C:\Program Files\MySQL\MySQL Server 8.0\bin
```
2. **Backup**
```
mysqldump -u root -p usefulwebapps > C:\MySQLBackup\usefulwebapps_YYYY_MM_DD.sql
```
3. **Restore**
```
mysql -u root -p usefulwebapps < C:\MySQLBackup\usefulwebapps_YYYY_MM_DD.sql
```

## Deploying to Ubuntu Server

1. Login to the Ubuntu server and shut down the Kestrel service
```
cd /etc/systemd/system
sudo systemctl stop kestrel-usefulwebapps.service
```
2. Transfer publish files from Windows using rsync via Windows Subsystem for Linux (WSL). Open Ubuntu terminal in publish root folder then run the below rsync command.
```
rsync -av * user@hostIP:/var/www/thedotnetwizard.com/html
```
3. Restart the service
```
sudo systemctl start kestrel-usefulwebapps.service
```
> Optionally, instead of restarting the service. Reboot the server after updates; the service will restart on boot.

## Running the App on Ubuntu

```
cd /var/www/thedotnetwizard.com/html
./UsefulWebApps
```

## Backup and Restore on Ubuntu

1. Navigate to folder with backups
2. **Backup**
```
sudo mysqldump usefulwebapps > usefulwebapps_YYYY_MM_DD.sql
```
3. **Restore**
```
sudo mysql usefulwebapps < usefulwebapps_YYYY_MM_DD.sql
```

	
## Transfer Database or Images from Ubuntu to Windows

1. Open WSL / Ubuntu terminal on Windows
2. Transfer database dump
```
rsync -av user@hostIp:/home/beefcake/UseFulWebAppsSqlDumps/usefulwebapps_YYYY_MM_DD.sql /mnt/c/Users/arogala/Documents/SqlDumpFromServer
```
3. Sync images from server to local development folder
```
rsync -av user@hostIp:/var/www/thedotnetwizard.com/html/wwwroot/images/ /mnt/c/Users/arogala/Documents/GitHub/UsefulWebApps/UsefulWebApps/wwwroot/images/
```


## 🪪 License

**UsefulWebApps** is licensed under the **GNU Affero General Public License v3.0 (AGPL-3.0)**.

You are free to use, modify, and distribute this software under the terms of the AGPL. If you modify and publicly distribute the software — including via a hosted service — you must make your source code available under the same license.

[License v3.0 (AGPL-3.0)](LICENSE.md)