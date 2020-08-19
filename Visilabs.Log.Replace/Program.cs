using System;
using System.Configuration;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using StreamWriter = System.IO.StreamWriter;

namespace Visilabs.Log.Replace
{
    class Program
    {
        private static string corrupted;
        private static string modify;
        static void Main(string[] args)
        {
            var appSettings = ConfigurationManager.AppSettings;
            corrupted = appSettings["Visilabs"];
            modify = appSettings["Modify"];

            DirectoryInfo startDir = new DirectoryInfo(corrupted);

            RecurseFileStructure recurseFileStructure = new RecurseFileStructure();
            recurseFileStructure.TraverseDirectory(startDir);

            Console.ReadLine();
        }

        public class RecurseFileStructure
        {
            public void TraverseDirectory(DirectoryInfo directoryInfo)
            {
                var subdirectories = directoryInfo.EnumerateDirectories();

                foreach (var subdirectory in subdirectories)
                {
                    TraverseDirectory(subdirectory);
                }

                var files = directoryInfo.EnumerateFiles();
                int counter = 0;
                foreach (var file in files)
                {
                    if (file.Name.EndsWith(".log"))
                    {
                        counter += 1;
                        var percentage = (counter * 100) / files.Count();
                        HandleFile(file);
                        Console.WriteLine("%" + percentage + " of files is done.");
                    }
                }
            }

            void HandleFile(FileInfo file)
            {
                Console.WriteLine("{0}", file.Name);
                try
                {
                    string[] lines = File.ReadAllLines(corrupted + file.Name);
                    using (StreamWriter writer = new StreamWriter(modify + file.Name))
                    {
                        Console.WriteLine("Process has started...");
                        for (int i = 1; i < lines.Length - 1; i++)
                        {
                            if (lines[i].Contains("OM.tid"))
                            {
                                var percentage = (i * 100) / lines.Count();
                                Console.WriteLine(i + "th line was corrupted at %" + percentage + " and recovered.");
                                lines[i] = lines[i].Replace("OM.pb", "OM.pp");
                            }
                            writer.WriteLine(lines[i]);
                        }
                        Console.WriteLine("Fully healed.");
                        writer.Flush();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
            }
        }
    }
}



