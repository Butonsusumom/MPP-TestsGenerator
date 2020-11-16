using System;
using System.IO;
using System.Threading.Tasks;

namespace TestsGenerator.IO
{
    public class AsyncFileReader : IAsyncReader
    {
        public async Task<string> ReadTextAsync(string path)
        {
            if (path == null)
            {
                throw new ArgumentException("Path shouldn't be null");
            }

            using (StreamReader reader = new StreamReader(path))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
