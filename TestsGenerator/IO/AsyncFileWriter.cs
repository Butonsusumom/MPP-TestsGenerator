using System;
using System.IO;
using System.Threading.Tasks;
using TestsGenerator.DataStructures;

namespace TestsGenerator.IO
{
    public class AsyncFileWriter : IAsyncWriter
    {
        protected string directory;

        public async Task WriteTextAsync(PathContentPair pathContentPair)
        {
            if (pathContentPair == null)
            {
                throw new ArgumentException("PathContent pair shouldn't be null");
            }
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            using (StreamWriter writer = new StreamWriter(Directory + Path.DirectorySeparatorChar + pathContentPair.Path))
            {
                await writer.WriteAsync(pathContentPair.Content);
            }
        }

        public string Directory
        {
            get => directory;
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("Directory shouldn't be null");
                }
                directory = Path.GetFullPath(value);
            }
        }

        public AsyncFileWriter()
        {
            Directory = System.IO.Directory.GetCurrentDirectory();
        }
    }
}
