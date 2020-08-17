using System;
using System.IO;
using System.Linq;
using StreamWriter = System.IO.StreamWriter;

namespace Visilabs.Log.Replace
{
    class Program
    {
        static void Main(string[] args)
        {

            string folderPath = "C:\\corruptedLogs";

            DirectoryInfo startDir = new DirectoryInfo(folderPath);

            RecurseFileStructure recurseFileStructure = new RecurseFileStructure();
            recurseFileStructure.TraverseDirectory(startDir);
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

                foreach (var file in files)
                {
                    HandleFile(file);
                }

            }

            void HandleFile(FileInfo file)
            {
                Console.WriteLine("{0}", file.Name);
                try
                {
                    string[] lines = File.ReadAllLines("C:\\corruptedLogs\\"+file.Name);
                    using (StreamWriter writer = new StreamWriter("C:\\Modify\\"+file.Name))
                    {
                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].Contains("OM.tid"))
                            {
                                lines[i] = lines[i].Replace("OM.pb", "OM.pp");
                            }
                            writer.WriteLine(lines[i]);
                        }
                        writer.Flush();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
                finally
                {
                    Console.WriteLine("Executing finally block.");
                }
            }
        }
    }
}



