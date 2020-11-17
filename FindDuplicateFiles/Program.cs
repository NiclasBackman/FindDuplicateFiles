using DuplicatesLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace FindDuplicateFiles
{
    class Program
    {
        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Finding duplicate files...");
            // Uncomment QueryDuplicates2 to run that query.  
            //QueryDuplicates();
            //QueryDuplicates2();  
            //QueryDuplicates3();
            var d = new DuplicateFinder();
            //@"C:\Users\Surface Pro\source\repos\FindDuplicateFiles\test");

            //var res = d.QueryDuplicates(new Myprogress(), );

            // Keep the console window open in debug mode.  
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            Console.WriteLine("...done!");
        }
        static void QueryDuplicates()
        {
            // Change the root drive or folder if necessary  
            string startFolder = @"C:\Users\Surface Pro\source\repos\FindDuplicateFiles\test";

            // Take a snapshot of the file system.  
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(startFolder);

            // This method assumes that the application has discovery permissions  
            // for all folders under the specified path.  
            IEnumerable<System.IO.FileInfo> fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

            // used in WriteLine to keep the lines shorter  
            int charsToSkip = startFolder.Length;

            // var can be used for convenience with groups.  
            var queryDupNames =
                from file in fileList
                group file.FullName.Substring(charsToSkip) by file.Name into fileGroup
                where fileGroup.Count() > 1
                select fileGroup;

            // Pass the query to a method that will  
            // output one page at a time.  
            PageOutput<string, string>(queryDupNames);
        }

        static void QueryDuplicates2()
        {
            // Change the root drive or folder if necessary.  
            //string startFolder = @"C:\Users\nicla\source\repos\FindDuplicateFiles\test";
            string startFolder = @"\\readynas\media\Pictures\Kort\Thailand 2019";

            // Make the lines shorter for the console display  
            int charsToSkip = startFolder.Length;

            // Take a snapshot of the file system.  
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(startFolder);
            IEnumerable<System.IO.FileInfo> fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

            // Note the use of a compound key. Files that match  
            // all three properties belong to the same group.  
            // A named type is used to enable the query to be  
            // passed to another method. Anonymous types can also be used  
            // for composite keys but cannot be passed across method boundaries  
            //
            var queryDupFiles =
                from file in fileList
                group file.FullName.Substring(charsToSkip) by
                    new PortableKey { Name = file.FullName, Hash = CalculateMD5(file.FullName) } into fileGroup
                where fileGroup.Count() > 1
                select fileGroup;

            var list = queryDupFiles.ToList();

            int i = queryDupFiles.Count();

            PageOutput<PortableKey, string>(queryDupFiles);
        }

        private static void drawTextProgressBar(int progress, int total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }
        private static string CalculateChecksum(string path, MD5 md5)
        {
                using (var stream = File.OpenRead(path))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).ToLowerInvariant();
                }
        }

        static void QueryDuplicates3()
        {
            string startFolder = @"C:\Users\Surface Pro\source\repos\FindDuplicateFiles\test";
            //string startFolder = @"C:\Users\Surface Pro\Desktop\Florida 2012";
            //string startFolder = @"\\readynas\media\Pictures\Kort";
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(startFolder);
            IEnumerable<System.IO.FileInfo> fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
            Console.WriteLine("{0} objects", fileList.Count());
            var md5 = MD5.Create();
            var objects = new List<FileEntry>();
            int cnt = 1;
            int max = fileList.Count();
            foreach (var f in fileList)
            {
                objects.Add(new FileEntry(f.FullName, CalculateChecksum(f.FullName, md5)));
                //Console.WriteLine("Processed {0} objects", cnt);
                drawTextProgressBar(cnt, max);
                cnt++;
            }
            var duplicates = objects.GroupBy(x => x.Checksum).Where(g => g.Skip(1).Any()).SelectMany(g => g);
            var distinct = duplicates.GroupBy(x => x.Checksum).Select(g => g.First().Checksum);
            Console.WriteLine("\n\n");

            using (StreamWriter outputFileStream = new StreamWriter(@".\duplicates.txt"))
            {
                foreach (var e in distinct)
                {
                    outputFileStream.WriteLine("Checksum {0} has following duplicates:", e);
                    var entries = objects.Where(x => x.Checksum == e);
                    foreach (var d in entries)
                    {
                        outputFileStream.WriteLine($"{d.Path}");
                    }
                    outputFileStream.WriteLine("\n");
                }
            }
        }

        // A generic method to page the output of the QueryDuplications methods  
        // Here the type of the group must be specified explicitly. "var" cannot  
        // be used in method signatures. This method does not display more than one  
        // group per page.  
        private static void PageOutput<K, V>(IEnumerable<System.Linq.IGrouping<K, V>> groupByExtList)
        {
            // Flag to break out of paging loop.  
            bool goAgain = true;

            // "3" = 1 line for extension + 1 for "Press any key" + 1 for input cursor.  
            int numLines = Console.WindowHeight - 3;

            // Iterate through the outer collection of groups.  
            foreach (var filegroup in groupByExtList)
            {
                // Start a new extension at the top of a page.  
                int currentLine = 0;

                // Output only as many lines of the current group as will fit in the window.  
                do
                {
                    Console.Clear();
                    Console.WriteLine("Filename = {0}", filegroup.Key.ToString() == String.Empty ? "[none]" : filegroup.Key.ToString());

                    // Get 'numLines' number of items starting at number 'currentLine'.  
                    var resultPage = filegroup.Skip(currentLine).Take(numLines);

                    //Execute the resultPage query  
                    foreach (var fileName in resultPage)
                    {
                        Console.WriteLine("\t{0}", fileName);
                    }

                    // Increment the line counter.  
                    currentLine += numLines;

                    // Give the user a chance to escape.  
                    Console.WriteLine("Press any key to continue or the 'End' key to break...");
                    ConsoleKey key = Console.ReadKey().Key;
                    if (key == ConsoleKey.End)
                    {
                        goAgain = false;
                        break;
                    }
                } while (currentLine < filegroup.Count());

                if (goAgain == false)
                    break;
            }
        }
    }
}
