using System.Threading.Tasks;

namespace TestsGenerator.IO
{
    public interface IAsyncReader
    {
        Task<string> ReadTextAsync(string path);
    }
}
