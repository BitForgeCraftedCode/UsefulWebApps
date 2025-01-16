# Usefulweb apps

Usefulweb apps is an Asp.Net Core MVC application with a MySQL server database. User logins are managed with [Identity Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-8.0&tabs=visual-studio) and [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) to connect to the database. All other data objects are mapped with [Dapper](https://github.com/DapperLib/Dapper) 

The Repository and Unit of Work Pattern is implemented in the application to separate data access logic from business/controller class logic.

The application is hosted on an Ubuntu 24.04 server and placed behind a reverse proxy Nginx server. I followed the documentation [here](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-8.0&tabs=linux-ubuntu) and [here](https://www.digitalocean.com/community/tutorials/how-to-deploy-an-asp-net-core-application-with-mysql-server-using-nginx-on-ubuntu-18-04). The publish profile is set up to be self contained so no need to install dot net on the server. 

The vision or purpose of this app is to be an open source self hosted family organizer application. For now it is just a recipe, list, and weather application but future plans are to add, family calendar, family chat, a website aggregator, and maybe a customized (per user) home page. Maybe add an email server to support a better login system and other fun things. 

## Build Directions local development

1. Install and configure MySQL Community Server Version 8.0.37 was used for my development on PC.
2. Clone and build the app with Visual Studio.
3. Seed the Database with the SQL dump file in the repository -- this is now really outdated and will no longer work (Too many data table changes behind). Thinking of a better way to do this. 
4. Run

## Back up and restore Windows

1. To back-up/restore use Windows command prompt not power shell
2. First open command promp in C:\Program Files\MySQL\MySQL Server 8.0\bin
3. To back-up: mysqldump -u root -p usefulwebapps > C:\MySQLBackup\usefulwebapps_2024_09_03.sql
4. To restore run: mysql -u root -p usefulwebapps < C:\MySQLBackup\usefulwebapps_2024_09_03.sql

The plan will be to have a development database with dummy users and data. Then you can login as admin change and change the PW before hosting. Then backup data base and host with new PW.

## Transfer publish files to Ubuntu server

1. First shut down the Kestrel server (the app)
2. login to Ubuntu server cd /etc/systemd/system
3. sudo systemctl stop kestrel-usefulwebapps.service
4. On the local pc I use Windows Subsystem for Linux
5. Open Ubuntu terminal on local pc in publish root folder then run this
6. scp -r * user@hostIP:/var/www/thedotnetwizard.com/html -- better off with rsync below
	* rsync -av * user@hostIP:/var/www/thedotnetwizard.com/html
7. After transefer complete restart the service 
8. sudo systemctl start kestrel-usefulwebapps.service
9. you could also just reboot the server and now may be a good time to update it.

## To run the app on Ubuntu server

1. Go to /var/www/thedotnetwizard.com/html and type ./UsefulWebApps

## Back up and restore from Ubuntu server

1. Navigate to folder with backups
2. sudo mysqldump usefulwebapps > backupfilename.sql
	* This will dump the usefulwebapps database in the backup folder
3. sudo mysql usefulwebapps < backupfilename.sql
	* This will restore the usefulwebapps database with the backup file.
	
## Transfer files from Ubuntu server to Windows

1. I use Windows Subsystem for Linux so open Ubuntu terminal
2. scp user@hostIP:/home/beefcake/UseFulWebAppsSqlDumps/usefulwebapps_2024_11_01.sql /mnt/c/Users/arogala/Documents/SqlDumpFromServer -- better off with rsync below
	* rsync -av user@hostIp:/home/beefcake/UseFulWebAppsSqlDumps/useful_2025_01_16_prod.sql /mnt/c/Users/arogala/Documents/SqlDumpFromServer
	* sync images on server to dev location
	* rsync -av user@hostIp:/var/www/thedotnetwizard.com/html/wwwroot/images/ /mnt/c/Users/arogala/Documents/GitHub/UsefulWebApps/UsefulWebApps/wwwroot/images/
3. SCP (secure copy) works like this
	* scp [OPTION] [user@]SRC_HOST:]file1 [user@]DEST_HOST:]file2
4. Don't need host IP if host is local PC

