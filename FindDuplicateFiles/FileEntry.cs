using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace FindDuplicateFiles
{
    class FileEntry
    {
        private string path;
        private string checksum;

        public FileEntry(string path, string checksum)
        {
            Path = path;
            Checksum = checksum;
        }

        public string Checksum { get => checksum; set => checksum = value; }

        public string Path { get => path; set => path = value; }
    }
}
