using System.Threading.Tasks;
using TestsGenerator.DataStructures;

namespace TestsGenerator.IO
{
    public interface IAsyncWriter
    {
        Task WriteTextAsync(PathContentPair pathContentPair);
    }
}
