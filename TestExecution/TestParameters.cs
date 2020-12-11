namespace test_execution.TestExecution
{
    public sealed class TestParameters
    {
        public string FwPath { get; set; }
        // CATS local installation path
        public const string CATS_INSTALLATION_LOCATION = @"C:\Cats";
        // Released versions of CATS
        public const string CATS_SOFTWARE_LOCATION = @"\\ws-fs1\Company\\Technical\Software\Cats";
        // Archived versions of CATS
        public const string CATS_ARCHIVED_LOCATION = @"\\WS-FS1\archive\Archive\Technical\Software\Cats";
        //7zip executable location
        public const string ZIP_PATH = @"C:\Program Files\7-Zip\7z.exe";
    }
}