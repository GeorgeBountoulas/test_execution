using System;
using System.Diagnostics;
using System.IO;


namespace test_execution.TestExecution
{
    public class CleanCats
    {
        public static void CleanInstalledCats(string catsFolder)
        {
            Console.WriteLine($"Deleting all files in {catsFolder}!");
           
                //Read all the files in the specified fodler
                string[] filePaths = Directory.GetFiles(catsFolder, "*.*", SearchOption.AllDirectories);
                foreach (string filePath in filePaths)
                {
                    var name = new FileInfo(filePath).Name;
                    name = name.ToLower();
                    if (!name.Contains(TestParameters.CATS_LICENSE_NAME))
                    {
                        try
                        {
                            File.Delete(filePath);
                            Console.WriteLine($"Deleting {name}");
                        }
                        catch (System.Exception excpt)
                        {
                            Console.WriteLine(excpt.Message);
                        }

                    }
                }
            
        }
    }
}