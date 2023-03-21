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
                        cmd.CommandText = "SELECT * FROM tabela1";
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Zapisz dane do pliku .sql
                            using (StreamWriter writer = new StreamWriter("backup_lif_server/tabela1_backup.sql"))
                            {
                                while (reader.Read())
                                {
                                    string values = "";
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        if (i > 0) values += ",";
                                        values += "'" + reader.GetValue(i).ToString().Replace("'", "''") + "'";
                                    }
                                    writer.WriteLine("INSERT INTO tabela1 VALUES(" + values + ");");
                                }
                            }
                        }
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Backup completed successfully.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred. {ex.Message}");
                Console.ResetColor();
            }
        }


    }
}
