using System;
using System.Collections.Generic;
using System.Linq;
using TestsGenerator.IO;

namespace TestsGenerator.UsageExample
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Console.WriteLine("Write list of test separating them by spaces");
            var testFiles = Console.ReadLine().Split(' ').ToList();
            Console.WriteLine("Write max amount of streams for reading");
            var maxInputStreams = int.Parse(Console.ReadLine());
            Console.WriteLine("Write max amount of streams for writing");
            var maxOutStreams = int.Parse(Console.ReadLine());
            Console.WriteLine("Write max amount of streams for generating tests");
            var maxMainStreams = int.Parse(Console.ReadLine());*/
            TestsGeneratorConfig config = new TestsGeneratorConfig
            {
                ReadPaths = new List<string>
                {
                    "../../SimpleTestFile.cs",
                    "../../ExtendedTestFile.cs"
                },
                Writer = new AsyncFileWriter()
                {
                    Directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                },
                ReadThreadCount = 2,
                WriteThreadCount = 2,
                processThreadCount = 2
                
                /*ReadPaths=testFiles,
                Writer = new AsyncFileWriter()
                {
                    Directory = "../../GeneratedTests"
                },
                ReadThreadCount = maxInputStreams,
                WriteThreadCount = maxOutStreams,
                processThreadCount = maxMainStreams*/
                
                    
            };

            new TestsGenerator(config).Generate().Wait();
            Console.WriteLine("Generation completed");
            Console.ReadKey();
        }
    }
}
