using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using SevenZip;
//using test_execution.TestExecution;
using Newtonsoft.Json;
using SevenZip.Compression.LZMA;

namespace test_execution.TestExecution
{
    public class InstallCats
    {

        private static string[] GetInstalledCatsVersions(string catsVersion, string searchPath)
        {
            string[] installedCatsVersions = Directory.GetDirectories(searchPath, $"*{catsVersion}*", SearchOption.TopDirectoryOnly);
            return installedCatsVersions;
        }

        private static string[] GetReleasedCatsVersions(string catsVersion, string searchPath)
        {
            string[] installedCatsVersions = Directory.GetFiles(searchPath, $"*{catsVersion}*", SearchOption.AllDirectories);
            return installedCatsVersions;
        }

        private static void ExtractCats(string archivePath, string destinationPath)
        {
            try
            {
                //get the archive name                
                string archiveName = Path.GetFileNameWithoutExtension(archivePath);
                Console.WriteLine($"Extracting {archiveName} from {archivePath}!");

                //SevenZip.Compression.LZMA.Decoder

                ProcessStartInfo pro = new ProcessStartInfo();
                pro.WindowStyle = ProcessWindowStyle.Hidden;
                pro.FileName = TestParameters.ZIP_PATH;
                pro.Arguments = string.Format($"x {archivePath} -y -o{destinationPath}\\{archiveName}");
                Process x = Process.Start(pro);
                x.WaitForExit();

                if (Directory.Exists($"{destinationPath}\\{archiveName}"))
                {
                
                    //ZipFile.ExtractToDirectory(catsPath, TestParameters.CATS_INSTALLATION_LOCATION);
                    Console.WriteLine($"CATS successfully extracted in '{destinationPath}\\{archiveName}'!");
                }
                else
                {
                    Console.WriteLine("Error extracting CATS!");
                }
            }

            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        public static string CatsInstallation(string catsVersion)
        {            
            string[] releasedCatsVersions;

            //Search if the CATS version is already installed 
            string[] installedCatsVersions = InstallCats.GetInstalledCatsVersions(catsVersion, TestParameters.CATS_INSTALLATION_LOCATION);
            if (installedCatsVersions.Length == 0)
            {
                Console.WriteLine($"CATS version v{catsVersion} is not installed!");
                Console.WriteLine($"Searching in {TestParameters.CATS_SOFTWARE_LOCATION}.");

                // search for CATS in the released folder if there are the required CATS version is not already installed
                releasedCatsVersions = InstallCats.GetReleasedCatsVersions(catsVersion, TestParameters.CATS_SOFTWARE_LOCATION);
                if (releasedCatsVersions.Length == 0)
                {
                    
                    Console.WriteLine($"Searching in {TestParameters.CATS_ARCHIVED_LOCATION}.");
                    // search for the required CATS in the archived folder
                    releasedCatsVersions = InstallCats.GetReleasedCatsVersions(catsVersion, TestParameters.CATS_ARCHIVED_LOCATION);

                    //Console.WriteLine($"The number of directories starting with '{catsVersion}' is {releasedCatsVersions.Length}.");
                    if (releasedCatsVersions.Length == 0)
                    {
                        Console.WriteLine($"***The requrested CATS version v{catsVersion} could not be found***");
                        Environment.Exit(1);
                    }
                    else
                    {
                        foreach (string dir in releasedCatsVersions)
                        {
                            Console.WriteLine(dir);
                            
                        }
                    }                    
                }
                else
                {
                    Console.WriteLine($"The number of directories starting with '{catsVersion}' is {releasedCatsVersions.Length}.");
                    foreach (string dir in releasedCatsVersions)
                    {
                        Console.WriteLine(dir);
                    }
                }

                //Extract CATS to the runner
                ExtractCats(releasedCatsVersions[0], TestParameters.CATS_INSTALLATION_LOCATION);

                //get the exact name of the folder that CATS was extracted and use it to copy the license file
                string folderName = Path.GetFileNameWithoutExtension(releasedCatsVersions[0]);

                // To copy the CATS license file to the file to another location 
                Console.WriteLine($"Copying CATS License to '{TestParameters.CATS_INSTALLATION_LOCATION}\\{folderName}\\Data\\'.");
                System.IO.File.Copy(TestParameters.CATS_LICENSE_LOCATION, $"{TestParameters.CATS_INSTALLATION_LOCATION}\\{folderName}\\Data\\Cats.lic", false);

                return $"{TestParameters.CATS_INSTALLATION_LOCATION}\\{folderName}\\";
            }
            else
            {
                // if the versions that match the requested CATS version are more than one then exit with code 1
                if (installedCatsVersions.Length>1)
                {
                    Console.WriteLine($"Found {installedCatsVersions.Length} instances of CATS version '{catsVersion}' installed on the runner.");
                    foreach (string dir in installedCatsVersions)
                    {
                        Console.WriteLine(dir);
                    }
                    Environment.Exit(1);
                }
                else //check if the license file exist in the Data folder
                {
                    Console.WriteLine($"CATS version {catsVersion} is already installed!");
                    if (!File.Exists($"{installedCatsVersions[0]}\\Data\\Cats.lic"))
                    {
                        Console.WriteLine("CATS License file NOT found! Copying license File.");
                        // To copy the CATS license file to the file to another location 
                        System.IO.File.Copy(TestParameters.CATS_LICENSE_LOCATION, $"{installedCatsVersions[0]}\\Data\\Cats.lic", false);
                    }

                }
                return installedCatsVersions[0];
            } 
            
        }

/*        private static void DecompressFileLZMA(string inFile, string outFolder)
        {

            string archiveName = Path.GetFileNameWithoutExtension(inFile);
            string outFile = $"{outFolder}\\{archiveName}";
            SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
            FileStream input = new FileStream(inFile, FileMode.Open, FileAccess.Read);
            FileStream output = new FileStream(outFile, FileMode.Create);

            // Read the decoder properties
            byte[] properties = new byte[5];
            input.Read(properties, 0, 5);

            // Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            input.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            coder.SetDecoderProperties(properties);
            coder.Code(input, output, input.Length, fileLength, null);
            output.Flush();
            output.Close();
        }*/


        public static void DecompressFileLZMA(string inFile, string outFile)
        {
            //string archiveName = Path.GetFileNameWithoutExtension(inFile);
            //string outFile = $"{outFolder}\\{archiveName}";
           

            using (FileStream input = new FileStream(inFile, FileMode.Open))
            {
                using (FileStream output = new FileStream(outFile, FileMode.Create))
                {
                    SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();

                    byte[] properties = new byte[5];
                    if (input.Read(properties, 0, 5) != 5)
                        throw (new Exception("input .lzma is too short"));
                    decoder.SetDecoderProperties(properties);

                    long outSize = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        int v = input.ReadByte();
                        if (v < 0)
                            throw (new Exception("Can't Read 1"));
                        outSize |= ((long)(byte)v) << (8 * i);
                    }
                    long compressedSize = input.Length - input.Position;

                    decoder.Code(input, output, compressedSize, outSize, null);
                }
            }
        }

        public static void CompressFileLZMA(string inFile, string outFile)
        {
            Int32 dictionary = 1 << 23;
            Int32 posStateBits = 2;
            Int32 litContextBits = 3; // for normal files
            // UInt32 litContextBits = 0; // for 32-bit data
            Int32 litPosBits = 0;
            // UInt32 litPosBits = 2; // for 32-bit data
            Int32 algorithm = 2;
            Int32 numFastBytes = 128;

            string mf = "bt4";
            bool eos = true;
            bool stdInMode = false;


            CoderPropID[] propIDs =  {
                CoderPropID.DictionarySize,
                CoderPropID.PosStateBits,
                CoderPropID.LitContextBits,
                CoderPropID.LitPosBits,
                CoderPropID.Algorithm,
                CoderPropID.NumFastBytes,
                CoderPropID.MatchFinder,
                CoderPropID.EndMarker
            };

            object[] properties = {
                (Int32)(dictionary),
                (Int32)(posStateBits),
                (Int32)(litContextBits),
                (Int32)(litPosBits),
                (Int32)(algorithm),
                (Int32)(numFastBytes),
                mf,
                eos
            };

            using (FileStream inStream = new FileStream(inFile, FileMode.Open))
            {
                using (FileStream outStream = new FileStream(outFile, FileMode.Create))
                {
                    SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
                    encoder.SetCoderProperties(propIDs, properties);
                    encoder.WriteCoderProperties(outStream);
                    Int64 fileSize;
                    if (eos || stdInMode)
                        fileSize = -1;
                    else
                        fileSize = inStream.Length;
                    for (int i = 0; i < 8; i++)
                        outStream.WriteByte((Byte)(fileSize >> (8 * i)));
                    encoder.Code(inStream, outStream, -1, -1, null);
                }
            }

        }
    }
}