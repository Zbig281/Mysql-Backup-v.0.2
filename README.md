# Mysql-Backup-v.0.2

This Is a basic program that retrieves current data from Mysql for the server Life is fudal your own and more. Currently out of the box.

## Instructions

To use this program, edit the file named config.txt, which is located in config/config.txt with the following parameters:

- `server`: server IP or `127.0.0.1`
- `port`: port number (`3306` is the default)
- `user`: username (`root` is the default)
- `password`: enter your password so that the program runs autobackup without asking for the password
- `database`: database name
- `startHour`: Currently it doesn't work (work in progress). It works only in such a way that when you set the hour it will make a backup at this current hour and then the program stops.
- `backupInterval`: Currently not fully operational (work in progress).
