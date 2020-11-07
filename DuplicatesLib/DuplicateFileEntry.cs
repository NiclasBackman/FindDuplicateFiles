namespace DuplicatesLib
{
    public class SingleFileEntry
    {
        public SingleFileEntry()
        {
        }

        public SingleFileEntry(string path, string checksum)
        {
            FilePath = path;
            Checksum = checksum;
        }

        public string FilePath { get; set; }

        public string Checksum { get; set; }
    }
}
