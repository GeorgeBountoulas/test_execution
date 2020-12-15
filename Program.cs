using System;
using System.Diagnostics;
using System.IO;
using test_execution.TestExecution;


namespace test_execution
{
    class Program
    {
        static void Main(string[] args)
        {
            //
            var catsFolder = InstallCats.CatsInstallation(args[0]);

            Console.WriteLine($"CATS {args[0]} is installed in '{catsFolder}'");

            //Delete all files in Data directrory and sub-directories
            CleanCats.CleanInstalledCats($"{catsFolder}\\Data");

            //Start CATS Server
            ProcessStartInfo pro = new ProcessStartInfo();
            pro.WindowStyle = ProcessWindowStyle.Hidden;
            pro.FileName = $"{catsFolder}\\CML.Cats.Server.Host.exe";
            Process x = Process.Start(pro);
            x.WaitForExit();

            

        }
    }

}
