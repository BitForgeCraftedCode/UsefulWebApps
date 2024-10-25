# Usefulweb apps

Usefulweb apps is an Asp.Net Core MVC application with a MySQL server database. User logins are managed with [Identity Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-8.0&tabs=visual-studio) and [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) to connect to the database. All other data objects are mapped with [Dapper](https://github.com/DapperLib/Dapper) 

The Repository and Unit of Work Pattern is implemented in the application to separate data access logic from business/controller class logic.

The application is hosted on an Ubuntu 24.04 server and placed behind a reverse proxy Nginx server. I followed the documentation [here](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-8.0&tabs=linux-ubuntu) and [here](https://www.digitalocean.com/community/tutorials/how-to-deploy-an-asp-net-core-application-with-mysql-server-using-nginx-on-ubuntu-18-04). The publish profile is set up to be self contained so no need to install dot net on the server. 

The vision or purpose of this app is to be an open source self hosted family organizer application. For now it is just a recipe and list application but future plans are to add weather, family calendar, family chat, a website aggregator, and maybe a customized (per user) home page. Maybe add an email server to support a better login system and other fun things. 

## Build Directions local development

1. Install and configure MySQL Community Server Version 8.0.37 was used for my development on PC.
2. Clone and build the app with Visual Studio.
3. Seed the Database with the SQL dump file in the repository.
4. Run

## Back up and restore Windows

1. to back-up/restore use Windows command prompt not power shell
2. first open command promp in C:\Program Files\MySQL\MySQL Server 8.0\bin
3. to back-up: mysqldump -u root -p usefulwebapps > C\MySQLBackup\usefulwebapps_2024_09_03.sql
4. to restore run: mysql -u root -p usefulwebapps < C:\MySQLBackup\usefulwebapps_2024_09_03.sql

the plan will be to have a development database with dummy users and data. Then you can login as admin change and change the PW before hosting. Then backup data base and host with new PW.

## Transfer publish files to Ubuntu server

1. I use Windows Subsystem for Linux
2. Open Ubuntu terminal in publish root folder then run this
3. scp -r * user@hostIP:/var/www/thedotnetwizard.com/html

## To run the app on Ubuntu server

1. Go to /var/www/thedotnetwizard.com/html and type ./UsefulWebApps

## Back up and restore from Ubuntu server