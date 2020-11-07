using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace DuplicatesLib
{
    public static class FileQueryExtension
    {
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");
            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            return files.Where(f => extensions.Contains(f.Extension));
        }
    }

    public class DuplicateFinder : IDuplicateFinder
    {
        //private readonly string m_path;
        //private readonly string m_filter;


        //public DuplicateFinder(string path, string filter = "*.*")
        //{
        //    m_path = path;
        //    m_filter = filter;
        //}

        public List<SingleFileEntry> QueryDuplicates(BackgroundWorker bgw, DoWorkEventArgs eventArgs, string path, string searchFilter = "*.*")
        {
            var res = new List<SingleFileEntry>();

            DirectoryInfo dir = new DirectoryInfo(path);
            IEnumerable<System.IO.FileInfo> fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
            var filter = searchFilter.Replace("*", "");
            string[] extensions = filter.Split(';');
            if(searchFilter != "*.*")
            {
                fileList = fileList.Where(f => extensions.Contains(f.Extension.ToLower()));
            }
            var md5 = MD5.Create();
            var objects = new List<FileEntry>();
            double cnt = 1;
            double max = fileList.Count();
            foreach (var f in fileList)
            {
                if(bgw.CancellationPending)
                {
                    bgw.ReportProgress(100, eventArgs);
                    eventArgs.Cancel = true;
                    return new List<SingleFileEntry>();
                }

                objects.Add(new FileEntry(f.FullName, CalculateChecksum(f.FullName, md5)));
                var prg = (cnt / max) * 100.0;
                bgw.ReportProgress((int)prg, eventArgs);
                cnt++;
            }
            var duplicates = objects.GroupBy(x => x.Checksum).Where(g => g.Skip(1).Any()).SelectMany(g => g);
            var distinct = duplicates.GroupBy(x => x.Checksum).Select(g => g.First().Checksum);
            Console.WriteLine("\n\n");

            foreach (var e in distinct)
            {
                var entries = objects.Where(x => x.Checksum == e);
                foreach (var d in entries)
                {
                    res.Add(new SingleFileEntry(d.Path, e));
                }
            }
            bgw.ReportProgress(100, eventArgs);
            return res;
        }
        
        private static string CalculateChecksum(string path, MD5 md5)
        {
            using (var stream = File.OpenRead(path))
            {
                return BitConverter.ToString(md5.ComputeHash(stream)).ToLowerInvariant();
            }
        }
    }
}
