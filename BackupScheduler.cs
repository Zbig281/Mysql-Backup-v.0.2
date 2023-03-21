﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MySql.Data.MySqlClient;

namespace BackupScheduler
{
    class Scheduler
    {
        private static string backupDirectory = "backup_lif_server";
        private static string lastBackupFile = "backup_lif_last.txt";

        public static void Run()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Backup scheduler started.");
            Console.ResetColor();
            try
            {
                // Wczytaj dane z pliku konfiguracyjnego
                string[] config = File.ReadAllLines("config/config.txt");
                int backupInterval = int.Parse(config[6].Substring(config[6].IndexOf('=') + 1));
                string[] startTime = config[5].Substring(config[5].IndexOf('=') + 1).Split(':');
                int startHour = int.Parse(startTime[0]);
                int startMinute = int.Parse(startTime[1]);

                // Wczytaj datę i godzinę ostatniego backupu z pliku
                DateTime lastBackupTime;
                if (File.Exists(lastBackupFile))
                {
                    string lastBackupTimeString = File.ReadAllText(lastBackupFile);
                    lastBackupTime = DateTime.Parse(lastBackupTimeString);
                }
                else
                {
                    // Jeśli plik nie istnieje, ustaw datę i godzinę na początek epoki (01/01/1970 00:00:00)
                    lastBackupTime = new DateTime(1970, 1, 1, 0, 0, 0);
                }

                while (true)
                {
                    DateTime now = DateTime.Now;

                    // Oblicz datę i godzinę następnego backupu
                    DateTime nextBackupTime = new DateTime(now.Year, now.Month, now.Day, startHour, startMinute, 0);
                    while (nextBackupTime < now)
                    {
                        nextBackupTime = nextBackupTime.AddDays(1);
                    }

                    TimeSpan waitTime = nextBackupTime - now;
                    Console.WriteLine($"Next backup scheduled for {nextBackupTime.ToShortTimeString()}, waiting for {waitTime.TotalMinutes} minutes... Press Escape to cancel.");

                    // Odliczanie czasu do kolejnego backupu
                    while (waitTime.TotalMinutes >= 1)
                    {
                        if (Console.KeyAvailable)
                        {
                            ConsoleKey key = Console.ReadKey(true).Key;
                            if (key == ConsoleKey.Escape)
                            {
                                Console.WriteLine("Backup aborted.");
                                return;
                            }
                        }

                        Thread.Sleep(TimeSpan.FromSeconds(5));
                        waitTime = nextBackupTime - DateTime.Now;
                        Console.WriteLine($"Waiting for {waitTime.Hours:00}:{waitTime.Minutes:00}... Press Escape to cancel.");
                    }

                    // Wykonaj backup
                    PerformBackup();

                    // Zapisz log
                    string logFilePath = Path.Combine(backupDirectory, "backup_log.txt");
                    File.AppendAllText(logFilePath, $"Backup performed at {DateTime.Now}\n");

                    // Poczekaj na kolejny backup
                    Thread.Sleep(backupInterval * 60 * 1000);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred. {ex.Message}");
                Console.ResetColor();
            }
        }

        private static void PerformBackup()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("!CREATE BACKUP NOW WAIT SEC!");
            Console.ResetColor();

            try
            {
                // Wczytaj dane z pliku konfiguracyjnego
                string[] config = File.ReadAllLines("config/config.txt");
                string ip = config[0].Substring(config[0].IndexOf('=') + 1);
                int port = int.Parse(config[1].Substring(config[1].IndexOf('=') + 1));
                string username = config[2].Substring(config[2].IndexOf('=') + 1);
                string password = config[3].Substring(config[3].IndexOf('=') + 1);
                string dbName = config[4].Substring(config[4].IndexOf('=') + 1);

                // Połącz się z bazą danych i wykonaj backup
                string connectionString = $"server={ip};port={port};user={username};password={password};database={dbName};";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        string argument = $"-h {ip} -u {username} -p{password} {dbName}";
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = "mysqldump",
                            Arguments = argument,
                            RedirectStandardOutput = true,
                            UseShellExecute = false
                        };
                        Process process = new Process { StartInfo = startInfo };
                        process.Start();
                        string backupDirectory = "backup_lif_server";
                        Directory.CreateDirectory(backupDirectory);
                        string backupPath = Path.Combine(backupDirectory, $"backup_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.sql");

                        using (StreamWriter sw = new StreamWriter(backupPath))
                        {
                            sw.Write(process.StandardOutput.ReadToEnd());
                        }
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Backup Done!");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred. Check that your MySQL login credentials are correct: {ex.Message}");
                Console.ResetColor();
            }
            finally
            {
                Console.WriteLine("FINISH!.");
                Scheduler.Run();
            }
        }
    }
}

