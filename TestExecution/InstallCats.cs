using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using SevenZip;
using test_execution.TestExecution;
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
                pro.Arguments = string.Format($"x {archivePath} -y -o{destinationPath}\\{archiveName}" );
                Process x = Process.Start(pro);
                x.WaitForExit();

                //ZipFile.ExtractToDirectory(catsPath, TestParameters.CATS_INSTALLATION_LOCATION);
                Console.WriteLine($"CATS successfully extracted in {destinationPath}\\{archiveName}!");
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        public static void CatsInstallation(string catsVersion)
        {
            //Search if the CATS is already installed in the PC
            string[] releasedCatsVersions;
            string[] installedCatsVersions = InstallCats.GetInstalledCatsVersions(catsVersion, TestParameters.CATS_INSTALLATION_LOCATION);
            if (installedCatsVersions.Length == 0)
            {
                Console.WriteLine($"CATS version v{catsVersion} is not installed!");
                Console.WriteLine($"Searching in {TestParameters.CATS_SOFTWARE_LOCATION}.");

                // search for CATS in the released folder
                releasedCatsVersions = InstallCats.GetReleasedCatsVersions(catsVersion, TestParameters.CATS_SOFTWARE_LOCATION);
                if (releasedCatsVersions.Length == 0)
                {
                    // search for the requested CATS in the archived folder
                    Console.WriteLine($"Searching in {TestParameters.CATS_ARCHIVED_LOCATION}.");

                    releasedCatsVersions = InstallCats.GetReleasedCatsVersions(catsVersion, TestParameters.CATS_ARCHIVED_LOCATION);

                    Console.WriteLine($"The number of directories starting with '{catsVersion}' is {releasedCatsVersions.Length}.");
                    foreach (string dir in releasedCatsVersions)
                    {
                        Console.WriteLine(dir);
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
                //DecompressFileLZMA(releasedCatsVersions[0], TestParameters.CATS_INSTALLATION_LOCATION);
                //DecompressFileLZMA("C:\\CML\\Cats-22.4.1.7z", "C:\\CML\\MITSystemArchitecture_test.pdf");
                //CompressFileLZMA("C:\\CML\\CATS_script_rework", "C:\\CML\\CATS_script_rework.7z");
            }
            else
            {
                Console.WriteLine($"The number of directories starting with '{catsVersion}' is {installedCatsVersions.Length}.");
                foreach (string dir in installedCatsVersions)
                {
                    Console.WriteLine(dir);
                }
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