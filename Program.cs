using System;
using DBBackupHelpers;
using BackupScheduler;

namespace DBBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Create by Zbig Brodaty v.0.0.2, !MAIN MENU!");
            Console.ResetColor();

            bool exit = false;

            while (!exit)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Do you want to:");
                Console.WriteLine("1. Perform backup now");
                Console.WriteLine("2. Schedule automatic backups - Currently not working 100%");
                Console.WriteLine("3. Exit");
                Console.ResetColor();

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        BackupHelperOne.PerformBackup();
                        break;
                    case "2":
                        Scheduler.Run();
                        break;
                    case "3":
                        exit = true;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid option, please choose again.");
                        Console.ResetColor();
                        break;
                }
            }
        }
    }
}
