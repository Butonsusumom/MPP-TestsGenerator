using System;
using System.Collections.Generic;
using TestsGenerator.CodeAnalysis;
using TestsGenerator.IO;
using TestsGenerator.TemplateGenerators;

namespace TestsGenerator
{
    public class TestsGeneratorConfig
    {
        protected int readThreadCount;
        public int processThreadCount;
        protected int writeThreadCount;
        protected IAsyncReader reader;
        protected IAsyncWriter writer;
        protected IEnumerable<string> readPaths;
        protected ITemplateGenerator templateGenerator;

        public int ReadThreadCount
        {
            get => readThreadCount;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("There should be at least 1 thread");
                }
                readThreadCount = value;
            }
        }

        public int ProcessThreadCount
        {
            get => processThreadCount;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("There should be at least 1 thread");
                }
                processThreadCount = value;
            }
        }

        public int WriteThreadCount
        {
            get => writeThreadCount;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("There should be at least 1 thread");
                }
                writeThreadCount = value;
            }
        }

        public IAsyncReader Reader
        {
            get => reader;
            set => reader = value ?? throw new ArgumentException("Reader shouldn't be null");
        }

        public IAsyncWriter Writer
        {
            get => writer;
            set => writer = value ?? throw new ArgumentException("Writer shouldn't be null");
        }

        public IEnumerable<string> ReadPaths
        {
            get => new List<string>(readPaths);
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("Paths shouldn't be null");
                }
                readPaths = new List<string>(value);
            }
        }

        public ITemplateGenerator TemplateGenerator
        {
            get => templateGenerator;
            set => templateGenerator = value ?? throw new ArgumentException("Template generator shouldn't be null");
        }

        public TestsGeneratorConfig()
        {
            readThreadCount = 1;
            writeThreadCount = 1;
            processThreadCount = Environment.ProcessorCount;
            reader = new AsyncFileReader();
            writer = new AsyncFileWriter();
            readPaths = new List<string>();
            templateGenerator = new TemplateGenerator(new CodeAnalyzer());
        }
    }
}
