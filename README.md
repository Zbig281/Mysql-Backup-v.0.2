# Mysql-Backup-v.0.2

This is a basic program that retrieves current data from MySQL for the server Life is Feudal: Your Own and more. Currently out of the box.

## Instructions  
To use this program, follow these steps:

1. Download or clone the project.
2. Add the `mysqldump.exe` file to the project directory.
3. Create a `config` folder inside the project directory and add the `config.txt` file to it.
4. Edit the `config.txt` file with the following parameters:

- `server`: server IP or `127.0.0.1`
- `port`: port number (`3306` is the default)
- `user`: username (`root` is the default)
- `password`: enter your password so that the program runs autobackup without asking for the password
- `database`: database name
- `startHour`: Currently it doesn't work (work in progress). It works only in such a way that when you set the hour it will make a backup at this current hour and then the program stops.
- `backupInterval`: Currently not fully operational (work in progress).
