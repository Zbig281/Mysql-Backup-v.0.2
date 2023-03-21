using System;
using System.IO;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace DBBackupHelpers
{
    public class BackupHelperOne
    {
        public static void PerformBackup()
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
                Environment.Exit(2);
            }
        }

    }
}
